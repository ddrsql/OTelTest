## 为什么

`TaggedTraceidCommandInterceptor.GetDrivingTableName` 当前只能通过 `FROM` 关键字匹配表名，无法识别 SQL 动作类型（SELECT/INSERT/UPDATE/DELETE），也无法正确提取 INSERT、UPDATE 语句中的目标表名。这导致 UPDATE 和 INSERT 操作在 OpenTelemetry Span 中的 DisplayName 为空或错误，降低了可观测性。

## 变更内容

- 重写 `GetDrivingTableName` 方法，按 SQL 动作类型分别提取驱动主表名
- 支持识别 SELECT（FROM）、INSERT（INTO）、UPDATE（UPDATE 后直接跟表名）、DELETE（FROM）四种操作
- 返回结构化的 `(Action, TableName)` 结果，替代当前的纯字符串拼接
- 更新 `ManipulateCommand` 中的调用方式，使 `activity.DisplayName` 格式为 `"SELECT AppUsers"` 风格
- 兼容 MySQL 反引号引用、方括号引用、无引用三种表名风格
- 添加单元测试覆盖各种 SQL 模式

## 功能 (Capabilities)

### 新增功能
- `sql-action-table-extraction`: 从 EF Core 生成的 SQL 语句中提取动作类型和驱动主表名的能力

### 修改功能

## 影响

- `TaggedTraceidCommandInterceptor.cs`：核心变更文件，方法签名和实现重写
- `ManipulateCommand` 方法：调用方式需适配新的返回结构
- 无 API 层面影响，变更限于内部实现
- 新增单元测试项目或在现有测试项目中添加测试类
