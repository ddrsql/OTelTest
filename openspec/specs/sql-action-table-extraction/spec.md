## 新增需求

### 需求:识别 SQL 动作类型
系统必须从 EF Core 生成的 SQL 语句首部识别出动作类型，支持的动作必须包含：SELECT、INSERT、UPDATE、DELETE。

#### 场景:识别 SELECT 语句
- **当** 输入 SQL 以 `SELECT` 开头
- **那么** 返回的动作必须为 `"SELECT"`

#### 场景:识别 INSERT 语句
- **当** 输入 SQL 以 `INSERT` 开头
- **那么** 返回的动作必须为 `"INSERT"`

#### 场景:识别 UPDATE 语句
- **当** 输入 SQL 以 `UPDATE` 开头
- **那么** 返回的动作必须为 `"UPDATE"`

#### 场景:识别 DELETE 语句
- **当** 输入 SQL 以 `DELETE` 开头
- **那么** 返回的动作必须为 `"DELETE"`

#### 场景:SQL 前有空白或注释
- **当** 输入 SQL 前部包含空白字符或 EF 注释前缀
- **那么** 系统必须跳过空白和注释，正确识别首个 SQL 关键字

### 需求:按动作类型提取驱动主表名
系统必须根据识别到的动作类型，使用对应策略提取驱动主表名：
- SELECT：从 `FROM` 关键字后提取第一张表名
- INSERT：从 `INTO` 关键字后提取表名
- UPDATE：从 `UPDATE` 关键字后直接提取表名
- DELETE：从 `DELETE FROM` 关键字后提取表名

#### 场景:SELECT 语句提取表名
- **当** SQL 为 `SELECT `a`.`Id` FROM `AppUsers` AS `a` WHERE `a`.`Id` = 1`
- **那么** 提取的表名必须为 `"AppUsers"`

#### 场景:SELECT 带 JOIN 只取驱动主表
- **当** SQL 为 `SELECT ... FROM `AppUsers` AS `a` INNER JOIN `AppBooks` AS `b` ON ...`
- **那么** 提取的表名必须为 `"AppUsers"`，禁止包含 JOIN 表

#### 场景:INSERT 语句提取表名
- **当** SQL 为 `INSERT INTO `AppUsers` (`Name`) VALUES (@p0)`
- **那么** 提取的表名必须为 `"AppUsers"`

#### 场景:UPDATE 语句提取表名
- **当** SQL 为 `UPDATE `AppUsers` SET `Name` = @p0 WHERE `Id` = @p1`
- **那么** 提取的表名必须为 `"AppUsers"`

#### 场景:DELETE 语句提取表名
- **当** SQL 为 `DELETE FROM `AppUsers` WHERE `Id` = @p0`
- **那么** 提取的表名必须为 `"AppUsers"`

### 需求:兼容多种表名引用风格
系统必须兼容以下表名引用格式：MySQL 反引号（`` `table` ``）、SQL Server 方括号（`[table]`）、无引用（`table`）。

#### 场景:反引号引用的表名
- **当** SQL 中表名为 `` `AppUsers` ``
- **那么** 提取的表名必须为 `"AppUsers"`（不含反引号）

#### 场景:方括号引用的表名
- **当** SQL 中表名为 `[AppUsers]`
- **那么** 提取的表名必须为 `"AppUsers"`（不含方括号）

#### 场景:无引用的表名
- **当** SQL 中表名为 `AppUsers`（无引用符号）
- **那么** 提取的表名必须为 `"AppUsers"`

### 需求:INSERT 多语句只取首条
系统在遇到 INSERT 后跟 SELECT（取自增 ID）的多语句场景时，必须只分析第一条语句的动作和表名。

#### 场景:INSERT 后跟 SELECT LAST_INSERT_ID
- **当** SQL 为 `INSERT INTO `AppUsers` (...) VALUES (...);\nSELECT `Id` FROM `AppUsers` WHERE ...`
- **那么** 动作必须为 `"INSERT"`，表名必须为 `"AppUsers"`

### 需求:返回结构化结果
方法必须返回包含 Action 和 Table 两个字段的结构化结果。

#### 场景:正常返回
- **当** SQL 为 `SELECT * FROM `Orders``
- **那么** 返回结果的 Action 必须为 `"SELECT"`，Table 必须为 `"Orders"`

#### 场景:无法识别的 SQL
- **当** 输入为 null、空字符串或无法匹配任何模式
- **那么** 返回结果的 Action 和 Table 必须为 null

### 需求:Span DisplayName 格式
`ManipulateCommand` 方法必须将 Span 的 DisplayName 设置为 `"ACTION TableName"` 格式。

#### 场景:SELECT 操作的 DisplayName
- **当** EF 生成 SELECT 查询 `SELECT ... FROM `AppBooks` ...`
- **那么** `activity.DisplayName` 必须为 `"SELECT AppBooks"`

#### 场景:INSERT 操作的 DisplayName
- **当** EF 生成 INSERT 语句 `INSERT INTO `AppUsers` ...`
- **那么** `activity.DisplayName` 必须为 `"INSERT AppUsers"`

#### 场景:无法解析时不覆盖 DisplayName
- **当** 解析结果中 Action 或 Table 为 null
- **那么** 禁止覆盖 `activity.DisplayName`
