## 新增需求

### 需求:每条 SQL 操作必须创建独立的子 Span
当 EF6 执行任意 SQL 命令（SELECT/INSERT/UPDATE/DELETE）时，拦截器必须通过 `ActivitySource.StartActivity()` 创建一个新的子 Span，禁止将 SQL 信息直接附加到父 Span 的 Tag 上。

#### 场景:查询操作创建子 Span
- **当** EF6 执行 `ExecuteReader()` 触发 `ReaderExecuting`
- **那么** 拦截器必须创建一个新的子 Activity，存储到 `ConcurrentDictionary<DbCommand, Activity>` 中

#### 场景:非查询操作创建子 Span
- **当** EF6 执行 `ExecuteNonQuery()` 触发 `NonQueryExecuting`
- **那么** 拦截器必须创建一个新的子 Activity，存储到 `ConcurrentDictionary<DbCommand, Activity>` 中

#### 场景:标量查询创建子 Span
- **当** EF6 执行 `ExecuteScalar()` 触发 `ScalarExecuting`
- **那么** 拦截器必须创建一个新的子 Activity，存储到 `ConcurrentDictionary<DbCommand, Activity>` 中

### 需求:SQL 执行完成后必须关闭子 Span
拦截器必须覆盖 `ReaderExecuted`、`NonQueryExecuted`、`ScalarExecuted` 方法，从字典中取出对应的 Activity 并调用 `Stop()`，然后从字典中移除。

#### 场景:正常执行后关闭 Span
- **当** SQL 命令执行完成触发 `*Executed` 方法
- **那么** 拦截器必须从字典中取出对应 Activity，调用 `Stop()`，并从字典中移除该条目

#### 场景:执行异常时标记 Error 并关闭 Span
- **当** SQL 命令执行完成，但 `InterceptionContext.Exception` 不为 null
- **那么** 拦截器必须在 Activity 上设置 `Status = Error` 和 `error.message` Tag，然后关闭并移除

### 需求:子 Span 的 DisplayName 必须显示 SQL 操作和表名
拦截器必须解析 SQL 语句，提取操作类型（SELECT/INSERT/UPDATE/DELETE）和驱动主表名，将子 Span 的 `DisplayName` 设置为 `{ACTION} {TABLE}` 格式（如 `SELECT AppUsers`）。

#### 场景:SELECT 语句解析
- **当** SQL 语句为 `SELECT * FROM AppUsers WHERE Id = 1`
- **那么** DisplayName 必须设置为 `SELECT AppUsers`

#### 场景:INSERT 语句解析
- **当** SQL 语句为 `INSERT INTO Tasks (Name) VALUES ('test')`
- **那么** DisplayName 必须设置为 `INSERT Tasks`

#### 场景:UPDATE 语句解析
- **当** SQL 语句为 `UPDATE Tasks SET Name = 'new' WHERE Id = 1`
- **那么** DisplayName 必须设置为 `UPDATE Tasks`

#### 场景:DELETE 语句解析
- **当** SQL 语句为 `DELETE FROM Tasks WHERE Id = 1`
- **那么** DisplayName 必须设置为 `DELETE Tasks`

#### 场景:无法解析的 SQL
- **当** SQL 语句无法匹配已知的操作模式（如 DDL、存储过程调用等）
- **那么** DisplayName 必须保持默认的拦截方法名（如 `ReaderExecuting`）

### 需求:SQL 语句必须注入 TraceId 注释
拦截器必须在原始 SQL 语句前注入 `/* TraceId:{traceId} {label} */` 格式的注释，其中 traceId 为子 Span 的 TraceId，label 为操作+表名。

#### 场景:注入 TraceId 注释
- **当** SQL 为 `SELECT * FROM AppUsers`
- **那么** 修改后的 CommandText 必须为 `/* TraceId:{traceId} SELECT AppUsers */ \n SELECT * FROM AppUsers`

### 需求:子 Span 必须记录 db.statement 属性
拦截器必须在子 Span 上设置 `db.statement` Tag，值为注入 TraceId 注释后的完整 SQL 语句。

#### 场景:记录 db.statement
- **当** 拦截器处理完 SQL 命令
- **那么** 子 Activity 必须包含 `db.statement` Tag，值为修改后的 CommandText

### 需求:拦截器必须在应用启动时注册一次
拦截器的注册必须通过 `DbInterception.Add()` 在 `OTelDataModule.PreInitialize()` 中执行，禁止在 `OnModelCreating` 中注册。

#### 场景:单次注册
- **当** 应用启动初始化 `OTelDataModule`
- **那么** 拦截器通过 `DbInterception.Add()` 注册一次

#### 场景:多个 DbContext 实例化
- **当** 应用运行期间多次创建 DbContext 实例
- **那么** 拦截器不会重复注册（`DbInterception` 中只有一个实例）
