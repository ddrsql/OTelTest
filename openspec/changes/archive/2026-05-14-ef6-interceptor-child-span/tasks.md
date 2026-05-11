## 1. 核心拦截器改造

- [x] 1.1 添加 `ConcurrentDictionary<DbCommand, Activity>` 字段和 `GetDrivingTableName()` / `ExtractMatch()` 方法到 `TaggedTraceidCommandInterceptor`
- [x] 1.2 重写 `ManipulateCommand` 方法：创建子 Activity、解析 SQL 设置 DisplayName、注入 TraceId 注释、设置 `db.statement` Tag、存入字典
- [x] 1.3 覆盖 `ReaderExecuted`、`NonQueryExecuted`、`ScalarExecuted` 方法：从字典取出 Activity，检查异常标记 Error，调用 Stop()，移除字典条目
- [x] 1.4 删除 `ManipulateCommand` 中的 else 分支（原无 Activity.Current 时的 fallback 逻辑）

## 2. 注册位置修复

- [x] 2.1 从 `OTelDbContext.OnModelCreating` 中移除 `DbInterception.Add(new TaggedTraceidCommandInterceptor())` 调用
- [x] 2.2 在 `OTelDataModule.PreInitialize` 中添加一次性注册 `DbInterception.Add(new TaggedTraceidCommandInterceptor())`

## 3. 验证

- [x] 3.1 编译确认无错误
- [ ] 3.2 运行应用，在 SigNoz 中确认 SQL 操作显示为独立子 Span，DisplayName 为 `SELECT AppUsers` 格式
