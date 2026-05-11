## 为什么

AbpFramework (EF6) 的 `TaggedTraceidCommandInterceptor` 将 SQL 信息作为 Tag 附加到 HTTP 父 Span 上，导致 SigNoz 中无法看到独立的 SQL 子 Span，Span Name 始终显示 HTTP 服务方法名而非 SQL 操作（如 `SELECT AppUsers`）。VoloAbp (EF Core) 版本已实现每条 SQL 作为独立子 Span，需要在 EF6 中达到同等追踪效果。

## 变更内容

- **改造 `TaggedTraceidCommandInterceptor`**：从"在父 Span 上打 Tag"模式改为"为每条 SQL 创建独立子 Span"模式
- **补全 `*Executed` 系列方法**：覆盖 `ReaderExecuted`、`NonQueryExecuted`、`ScalarExecuted`，在 SQL 执行完成后关闭子 Span，准确记录执行耗时
- **移植 `GetDrivingTableName()`**：从 VoloAbp 版本移植 SQL 解析逻辑，提取操作类型（SELECT/INSERT/UPDATE/DELETE）和驱动主表名
- **设置 `Activity.DisplayName`**：将子 Span 名称设置为 `SELECT AppUsers` 等格式，替代默认的方法名
- **异常状态标记**：在 `*Executed` 中检查 `InterceptionContext.Exception`，标记失败 Span
- **修复注册位置**：从 `OnModelCreating`（每次 DbContext 实例化重复注册）移至 `OTelDataModule.PreInitialize`（应用启动时注册一次）

## 功能 (Capabilities)

### 新增功能
- `ef6-sql-child-span`: EF6 拦截器为每条 SQL 操作创建独立的 OpenTelemetry 子 Span，包含完整生命周期管理（Executing 创建 → Executed 关闭）、SQL 解析、DisplayName 设置和异常标记

### 修改功能

（无现有规范需要修改）

## 影响

- **代码文件**：`TaggedTraceidCommandInterceptor.cs`（主要改造）、`OTelDbContext.cs`（移除注册代码）、`OTelDataModule.cs`（添加注册代码）
- **依赖项**：无新依赖，仅使用 `System.Diagnostics`（OpenTelemetry API）和 `System.Collections.Concurrent`（ConcurrentDictionary）
- **追踪数据**：SigNoz 中将出现新的 SQL 子 Span，Span Name 从 HTTP 方法名变为 SQL 操作+表名
- **兼容性**：无破坏性变更，对业务逻辑透明
