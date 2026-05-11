## 上下文

当前 AbpFramework 使用 EF6 (`System.Data.Entity`) 的 `DbCommandInterceptor` 拦截 SQL 命令。现有实现在 `ManipulateCommand` 中仅将 SQL 作为 Tag 附加到 `Activity.Current`（即 HTTP 请求的父 Span），不会创建独立的子 Span。这导致 SigNoz 追踪界面中：
- Span Name 显示 HTTP 方法名（如 `GET /api/tasks`）
- SQL 语句只在 Attributes 中可见
- 无法看到单条 SQL 的独立执行耗时

VoloAbp (EF Core) 版本通过 EF Core 内置的 OTel 集成自动创建 SQL 子 Span，再由拦截器设置 `DisplayName` 和 `db.statement`。EF6 没有内置 OTel 集成，需要手动管理子 Span 的完整生命周期。

EF6 的 `DbCommandInterceptor` 提供 6 个方法：`ReaderExecuting/Executed`、`NonQueryExecuting/Executed`、`ScalarExecuting/Executed`，成对的 Executing/Executed 保证了能在 SQL 执行前后分别介入。

## 目标 / 非目标

**目标：**
- 为每条 SQL 操作创建独立的 OpenTelemetry 子 Span
- 子 Span 的 DisplayName 显示为 `SELECT AppUsers` 格式（操作类型 + 主表名）
- 准确记录 SQL 执行耗时（Executing 创建 Span → Executed 关闭 Span）
- SQL 执行异常时标记 Span 为 Error 状态
- 修复拦截器重复注册问题

**非目标：**
- 不改造为 EF Core（框架选型不在本次范围）
- 不实现异步拦截方法（EF6 的 `DbCommandInterceptor` 本身无异步 API，异步操作内部走同步拦截路径）
- 不引入新的外部依赖

## 决策

### 决策 1：使用 ConcurrentDictionary\<DbCommand, Activity\> 传递 Activity 引用

在 `*Executing` 中创建子 Activity，需要将引用传递到对应的 `*Executed` 中关闭。

**选择方案**：`ConcurrentDictionary<DbCommand, Activity>`，以 `DbCommand` 对象引用作为 key。

**替代方案**：
- `AsyncLocal<Activity>`：同一请求中可能有嵌套 SQL 调用，AsyncLocal 只能保存一个值，栈模式心智负担大
- `DbCommand` 属性注入：EF6 的 `DbCommand` 没有用户状态字典，不可行

**理由**：EF6 保证同一个 `DbCommand` 实例在 Executing 和 Executed 中是同一引用，ConcurrentDictionary 天然线程安全，实现简洁。

### 决策 2：ActivitySource 命名沿用现有常量

使用 `OTelModule.AspNetSourceName`（`"OpenTelemetry.Instrumentation.AspNet.Telemetry"`）作为 ActivitySource 名称，与现有 ASP.NET 请求 Span 保持同一 Source，确保在 SigNoz 中归属于同一个 Service。

### 决策 3：GetDrivingTableName 直接移植

从 VoloAbp 版本移植 `GetDrivingTableName()` 方法，该方法是纯字符串正则解析，不依赖 EF Core 特定 API，EF6 的 SQL 语法完全兼容。

### 决策 4：注册位置移至 PreInitialize

将 `DbInterception.Add(new TaggedTraceidCommandInterceptor())` 从 `OTelDbContext.OnModelCreating` 移至 `OTelDataModule.PreInitialize`，保证应用启动时只注册一次，避免每次 DbContext 实例化时重复累积拦截器。

## 风险 / 权衡

**[Activity 泄漏]** → 如果 `*Executed` 因极端原因未被调用，Activity 会留在字典中。缓解：EF6 拦截器契约保证成对调用，正常情况不会发生。可在 `*Executed` 中使用 try-finally 确保清理。

**[DisplayName 覆盖父 Span]** → 如果错误地修改 `Activity.Current` 的 DisplayName，会改变 HTTP Span 的名称。缓解：通过 `_activitySource.StartActivity()` 创建的是新的子 Activity，其 DisplayName 修改不影响父 Span。

**[并发请求同一 DbCommand]** → 理论上不同请求可能使用不同 DbCommand 实例。ConcurrentDictionary 以引用为 key，不同实例不会冲突。
