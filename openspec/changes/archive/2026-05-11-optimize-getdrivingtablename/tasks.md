## 1. 核心方法重写

- [x] 1.1 重写 `GetDrivingTableName` 方法，返回 `(string Action, string Table)` 元组，实现两步正则策略：先识别 SQL 动作关键字，再按动作类型提取驱动主表名
- [x] 1.2 兼容三种表名引用风格：MySQL 反引号 `` `table` ``、SQL Server 方括号 `[table]`、无引用 `table`
- [x] 1.3 处理 INSERT 多语句场景：截取到第一个分号再分析，避免被后续 SELECT 干扰
- [x] 1.4 处理 null/空字符串输入，返回 `(null, null)`

## 2. 调用方适配

- [x] 2.1 更新 `ManipulateCommand` 方法，解构新的返回值 `(action, table)`
- [x] 2.2 将 `activity.DisplayName` 设置为 `"{action} {table}"` 格式（如 `"SELECT AppUsers"`）
- [x] 2.3 更新 SQL 注释注入格式为 `/* TraceId:{traceId} {action} {table} */`
- [x] 2.4 当 action 或 table 为 null 时，跳过 DisplayName 赋值，保持原注释格式

## 3. 单元测试

- [x] 3.1 在测试项目中添加 `GetDrivingTableName` 单元测试类
- [x] 3.2 测试 SELECT 简单查询（含反引号、方括号、无引用）
- [x] 3.3 测试 SELECT 带 JOIN（只取驱动主表）
- [x] 3.4 测试 INSERT INTO 提取表名
- [x] 3.5 测试 UPDATE 提取表名
- [x] 3.6 测试 DELETE FROM 提取表名
- [x] 3.7 测试 INSERT 后跟 SELECT LAST_INSERT_ID 的多语句场景
- [x] 3.8 测试 null 和空字符串输入
- [x] 3.9 所有测试通过
