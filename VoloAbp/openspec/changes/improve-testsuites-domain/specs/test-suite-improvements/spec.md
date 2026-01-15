# Spec: TestSuite Domain Improvements

## 新增需求

#### 场景：重置测试集状态
- **Given** 一个状态为 `Ready` 的测试集
- **When** 调用 `ResetToDraft()` 方法
- **Then** 测试集的状态应变更为 `Draft`
- **And** 如果状态不是 `Ready`，应抛出 `InvalidOperationException`

#### 场景：归档草稿测试集
- **Given** 一个状态为 `Draft` 的测试集
- **When** 调用 `Archive()` 方法
- **Then** 测试集的状态应变更为 `Archived`

#### 场景：记录执行时间
- **Given** 一个测试集
- **When** 调用 `Execute()` 方法
- **Then** `ExecutionStartTime` 应被设置为当前时间
- **And** `ExecutionEndTime` 应被重置为 `null`

#### 场景：完成执行记录时间
- **Given** 一个正在运行的测试集
- **When** 调用 `CompleteExecution()` 或 `FailExecution()` 方法
- **Then** `ExecutionEndTime` 应被设置为当前时间

#### 场景：防止重复标题更新
- **Given** 一个包含测试用例 "Case A" 和 "Case B" 的测试集
- **When** 尝试将 "Case A" 的标题更新为 "Case B"
- **Then** 应抛出 `InvalidOperationException` 提示标题已存在

#### 场景：导入测试用例反馈
- **Given** 一个测试集和一组待导入的测试用例（包含有效和无效数据）
- **When** 调用 `ImportTestCasesAsync`
- **Then** 应返回一个 `ImportResult` 对象
- **And** `ImportResult` 应包含成功数量和失败数量
- **And** `ImportResult` 应包含失败项的详细错误信息

## 修改需求

#### 场景：ImportTestCasesAsync 返回值
- **Given** `ITestSuiteManager` 接口
- **When** 调用 `ImportTestCasesAsync`
- **Then** 方法签名应从 `Task` 变为 `Task<TestCaseImportResult>`

#### 场景：TestSuite 属性
- **Given** `TestSuite` 类
- **Then** 应包含 `ExecutionStartTime` 和 `ExecutionEndTime` 属性
- **And** 应移除或保留 `LastExecutionTime`（视作 `ExecutionStartTime` 的别名或废弃）-> *决定保留 `LastExecutionTime` 保持兼容，但逻辑上等同于 StartTime*

## 文档需求

#### 场景：添加XML注释
- **Given** Domain层的所有公共类和方法 (`TestSuite`, `TestCase`, `TestSuiteManager`, etc.)
- **Then** 应包含清晰的 `<summary>` XML 注释
- **And** 方法参数应有 `<param>` 注释
- **And** 返回值应有 `<returns>` 注释
- **And** 异常应有 `<exception>` 注释
