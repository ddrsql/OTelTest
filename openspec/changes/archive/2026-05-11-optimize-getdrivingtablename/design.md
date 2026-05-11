## 上下文

`TaggedTraceidCommandInterceptor` 是一个 EF Core `DbCommandInterceptor`，在每次数据库命令执行前拦截 SQL，将 TraceId 和表名注入 SQL 注释，并设置 OpenTelemetry Span 的 DisplayName。

当前 `GetDrivingTableName` 仅通过 `FROM` 关键字正则匹配，存在以下缺陷：
- 无法识别 SQL 动作类型（SELECT/INSERT/UPDATE/DELETE）
- UPDATE 语句没有 FROM 关键字，无法提取表名
- INSERT 语句的表名在 INTO 关键字后，也无法匹配
- 返回所有 FROM 表的拼接字符串，无法区分驱动主表

项目使用 MySQL（Pomelo/Oracle provider），EF Core 生成的 SQL 使用反引号引用表名。测试环境使用 SQLite。

## 目标 / 非目标

**目标：**
- 正确识别 SQL 动作：SELECT、INSERT、UPDATE、DELETE
- 按动作类型提取驱动主表名
- 兼容 MySQL 反引号、方括号、无引用三种表名风格
- DisplayName 格式为 `"ACTION TableName"`
- 添加单元测试覆盖主要 SQL 模式

**非目标：**
- 不处理存储过程或原始手写 SQL（仅针对 EF Core 生成的 SQL）
- 不解析 CTE、嵌套子查询中的深层表引用
- 不修改 `TaggedTraceidCommandInterceptor` 以外的类
- 不处理批量多语句（仅分析第一条 SQL 语句）

## 决策

### 决策 1：返回值用值元组 `(string Action, string Table)`

**选择：** 使用 C# ValueTuple `(string Action, string Table)` 作为返回类型。

**替代方案：** 自定义 record/struct `SqlParseResult`。

**理由：** 该结构只在类内部使用，不需要额外类型定义。ValueTuple 轻量且支持解构。如果未来需要扩展字段，再重构为 record。

### 决策 2：两步正则策略——先识别动作，再按动作提表名

**选择：** 第一步用正则匹配 SQL 首关键字确定动作，第二步根据动作类型用对应正则提取表名。

```
Step 1: ^\s*(SELECT|INSERT|UPDATE|DELETE)\b
Step 2:
  SELECT → (?i)FROM\s+[`[]?(\w+)[`\]]?
  INSERT → (?i)INTO\s+[`[]?(\w+)[`\]]?
  UPDATE → (?i)UPDATE\s+[`[]?(\w+)[`\]]?
  DELETE → (?i)DELETE\s+FROM\s+[`[]?(\w+)[`\]]?
```

**替代方案：** 单一正则 `(?<action>SELECT|INSERT|UPDATE|DELETE)\s+(INTO\s+|FROM\s+)?(?<table>...)` 合并匹配。

**理由：** 单一正则在 UPDATE（无 FROM/INTO）场景下需要复杂的可选分支，可读性差。两步策略每个分支简单明确，易于维护和扩展。性能开销可忽略（正则本身已缓存编译）。

### 决策 3：方法可见性保持 public

**选择：** 保持 `GetDrivingTableName` 为 `public`，便于单元测试直接调用。

**理由：** 已是 public 方法，保持不变。测试项目可直接引用测试，无需 InternalsVisibleTo。

## 风险 / 权衡

**正则解析 SQL 的固有局限** → EF Core 生成的 SQL 格式稳定且可预测，正则方案对这类结构化 SQL 足够可靠。若未来 EF Core 大幅改变 SQL 生成格式，需相应调整正则。缓解措施：通过单元测试固定预期输出，格式变化时测试会失败提醒。

**多语句批处理** → EF Core 7+ 支持 SaveChanges batching，可能生成多语句。当前只分析第一条语句，覆盖绝大多数场景。缓解措施：在正则匹配前截取到第一个分号位置。

**Schema 限定表名（如 `dbo.AppUsers`）** → 当前 `(\w+)` 不匹配点号。缓解措施：使用 `(\w+(?:\.\w+)?)` 或在匹配后取最后一段。
