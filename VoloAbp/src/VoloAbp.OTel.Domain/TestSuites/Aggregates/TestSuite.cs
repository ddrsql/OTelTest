using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Aggregates;

/// <summary>
/// 聚合根：测试集
/// </summary>
public class TestSuite : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 测试集名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 项目标识
    /// </summary>
    public string ProjectKey { get; private set; }

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; private set; }

    /// <summary>
    /// 测试配置
    /// </summary>
    public TestConfiguration Configuration { get; private set; }

    /// <summary>
    /// 状态
    /// </summary>
    public TestSuiteStatus Status { get; private set; } = TestSuiteStatus.Draft;

    /// <summary>
    /// 总测试用例数
    /// </summary>
    public int TotalTestCases => _testCases.Count;

    /// <summary>
    /// 通过的测试用例数
    /// </summary>
    public int PassedTestCases => _testCases.Count(tc => tc.Status == TestCaseStatus.Passed);

    /// <summary>
    /// 失败的测试用例数
    /// </summary>
    public int FailedTestCases => _testCases.Count(tc => tc.Status == TestCaseStatus.Failed);

    /// <summary>
    /// 成功率 (0-100)
    /// </summary>
    public double SuccessRate => TotalTestCases > 0 ? (double)PassedTestCases / TotalTestCases * 100 : 0;

    /// <summary>
    /// 最后一次执行时间（通常等于 ExecutionStartTime）
    /// </summary>
    public DateTime? LastExecutionTime { get; private set; }

    /// <summary>
    /// 执行开始时间
    /// </summary>
    public DateTime? ExecutionStartTime { get; private set; }

    /// <summary>
    /// 执行结束时间
    /// </summary>
    public DateTime? ExecutionEndTime { get; private set; }

    /// <summary>
    /// 平均执行耗时
    /// </summary>
    public TimeSpan? AverageExecutionTime { get; private set; }

    // 聚合内的子实体集合
    private readonly List<TestCase> _testCases;
    public IReadOnlyCollection<TestCase> TestCases => _testCases.AsReadOnly();

    private TestSuite()
    {
        // for EF Core
        _testCases = new List<TestCase>();
    }

    internal TestSuite(
        Guid id,
        string name,
        string description,
        string projectKey,
        string version,
        TestConfiguration configuration)
        : base(id)
    {
        SetName(name);
        SetDescription(description);
        SetProjectKey(projectKey);

        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("版本号不能为空", nameof(version));
        Version = version.Trim();

        Configuration = configuration ?? new TestConfiguration(30, 3, false, "Development");
        _testCases = new List<TestCase>();
    }

    // 领域方法
    /// <summary>
    /// 添加新的测试用例到测试集。
    /// </summary>
    /// <param name="title">测试用例标题，在测试集中必须唯一。</param>
    /// <param name="description">测试用例描述。</param>
    /// <param name="steps">测试步骤。</param>
    /// <param name="expectedResult">预期结果。</param>
    /// <param name="priority">优先级，默认为 Medium。</param>
    /// <exception cref="InvalidOperationException">如果测试集已归档或正在运行，或标题已存在。</exception>
    public void AddTestCase(
        string title,
        string description,
        string steps,
        string expectedResult,
        TestPriority priority = null)
    {
        if (Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能添加测试用例");

        if (Status == TestSuiteStatus.Running)
            throw new InvalidOperationException("正在执行的测试集不能添加测试用例");

        // 业务规则：标题在同一测试集中不能重复
        if (_testCases.Any(tc => tc.Title.Equals(title.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"已存在标题为 '{title}' 的测试用例");

        var newTestCase = new TestCase(
            Guid.NewGuid(),
            title,
            description,
            steps,
            expectedResult,
            priority
        );

        _testCases.Add(newTestCase);
    }

    /// <summary>
    /// 从测试集中移除指定的测试用例。
    /// </summary>
    /// <param name="testCaseId">要移除的测试用例ID。</param>
    /// <exception cref="InvalidOperationException">如果测试集已归档。</exception>
    /// <exception cref="KeyNotFoundException">如果未找到指定的测试用例。</exception>
    public void RemoveTestCase(Guid testCaseId)
    {
        if (Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能删除测试用例");

        var testCase = _testCases.FirstOrDefault(tc => tc.Id == testCaseId);
        if (testCase == null)
            throw new KeyNotFoundException($"未找到ID为 {testCaseId} 的测试用例");

        _testCases.Remove(testCase);
    }

    /// <summary>
    /// 更新指定测试用例的详细信息。
    /// </summary>
    /// <param name="testCaseId">测试用例ID。</param>
    /// <param name="title">新标题（可选）。</param>
    /// <param name="description">新描述（可选）。</param>
    /// <param name="steps">新步骤（可选）。</param>
    /// <param name="expectedResult">新预期结果（可选）。</param>
    /// <param name="priority">新优先级（可选）。</param>
    /// <exception cref="KeyNotFoundException">如果未找到指定的测试用例。</exception>
    /// <exception cref="InvalidOperationException">如果测试集已归档。</exception>
    public void UpdateTestCase(
        Guid testCaseId,
        string title = null,
        string description = null,
        string steps = null,
        string expectedResult = null,
        TestPriority priority = null)
    {
        var testCase = _testCases.FirstOrDefault(tc => tc.Id == testCaseId);
        if (testCase == null)
            throw new KeyNotFoundException($"未找到ID为 {testCaseId} 的测试用例");

        if (Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能修改测试用例");

        if (title != null && !title.Equals(testCase.Title, StringComparison.OrdinalIgnoreCase))
        {
            if (_testCases.Any(tc => tc.Id != testCaseId && tc.Title.Equals(title.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"已存在标题为 '{title}' 的测试用例");
        }

        testCase.UpdateDetails(
            title ?? testCase.Title,
            description ?? testCase.Description,
            steps ?? testCase.Steps,
            expectedResult ?? testCase.ExpectedResult,
            priority ?? testCase.Priority
        );
    }

    /// <summary>
    /// 获取指定的测试用例。
    /// </summary>
    /// <param name="testCaseId">测试用例ID。</param>
    /// <returns>找到的测试用例。</returns>
    /// <exception cref="KeyNotFoundException">如果未找到指定的测试用例。</exception>
    public TestCase GetTestCase(Guid testCaseId)
    {
        return _testCases.FirstOrDefault(tc => tc.Id == testCaseId)
            ?? throw new KeyNotFoundException($"未找到ID为 {testCaseId} 的测试用例");
    }

    /// <summary>
    /// 开始执行测试集。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果测试集状态不是 Ready 或 Draft，或者没有启用的测试用例。</exception>
    public void Execute()
    {
        if (Status != TestSuiteStatus.Ready && Status != TestSuiteStatus.Draft)
            throw new InvalidOperationException($"测试集状态为 '{Status}'，无法执行");

        var enabledCases = _testCases.Where(tc => tc.IsEnabled).ToList();
        if (!enabledCases.Any())
            throw new InvalidOperationException("没有启用的测试用例用于执行");

        Status = TestSuiteStatus.Running;
        LastExecutionTime = DateTime.UtcNow;
        ExecutionStartTime = LastExecutionTime;
        ExecutionEndTime = null;

        // 将所有启用的测试用例标记为待执行
        foreach (var testCase in enabledCases)
        {
            testCase.MarkAsPending();
        }
    }

    /// <summary>
    /// 记录测试用例的执行结果。
    /// </summary>
    /// <param name="testCaseId">测试用例ID。</param>
    /// <param name="status">执行状态。</param>
    /// <param name="executionDuration">执行耗时。</param>
    /// <param name="errorMessage">错误信息（如果有）。</param>
    /// <param name="actualResult">实际结果（可选）。</param>
    /// <exception cref="InvalidOperationException">如果测试集未在运行中。</exception>
    public void RecordTestCaseResult(
        Guid testCaseId,
        TestCaseStatus status,
        TimeSpan? executionDuration = null,
        string errorMessage = null,
        string actualResult = null)
    {
        if (Status != TestSuiteStatus.Running)
            throw new InvalidOperationException("只能在测试集执行过程中记录测试用例结果");

        var testCase = GetTestCase(testCaseId);

        if (status == TestCaseStatus.Passed || status == TestCaseStatus.Failed)
        {
            testCase.RecordExecutionResult(executionDuration, errorMessage);
        }

        if (!string.IsNullOrEmpty(actualResult))
        {
            testCase.UpdateActualResult(actualResult);
        }

        // 检查是否所有测试用例都已完成
        CheckAndUpdateCompletion();
    }

    /// <summary>
    /// 标记测试集执行完成。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果测试集未在运行中。</exception>
    public void CompleteExecution()
    {
        if (Status != TestSuiteStatus.Running)
            throw new InvalidOperationException("只有运行中的测试集可以标记为完成");

        Status = TestSuiteStatus.Completed;
        ExecutionEndTime = DateTime.UtcNow;
        UpdateAverageExecutionTime();
    }

    /// <summary>
    /// 标记测试集执行失败。
    /// </summary>
    /// <param name="reason">失败原因。</param>
    /// <exception cref="InvalidOperationException">如果测试集未在运行中。</exception>
    public void FailExecution(string reason)
    {
        if (Status != TestSuiteStatus.Running)
            throw new InvalidOperationException("只有运行中的测试集可以标记为失败");

        Status = TestSuiteStatus.Completed;
        ExecutionEndTime = DateTime.UtcNow;
        UpdateAverageExecutionTime();

        // 标记所有未完成的测试用例为失败
        foreach (var testCase in _testCases.Where(tc => tc.Status == TestCaseStatus.Pending))
        {
            testCase.RecordExecutionResult(null, $"测试集执行失败: {reason}");
        }
    }

    /// <summary>
    /// 归档测试集。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果测试集未完成、未就绪或未处于草稿状态。</exception>
    public void Archive()
    {
        if (Status != TestSuiteStatus.Completed && Status != TestSuiteStatus.Ready && Status != TestSuiteStatus.Draft)
            throw new InvalidOperationException("只有已完成、就绪或草稿状态的测试集可以被归档");

        Status = TestSuiteStatus.Archived;
    }

    /// <summary>
    /// 将测试集标记为就绪状态。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果测试集不是 Draft 状态，或者没有测试用例。</exception>
    public void MarkAsReady()
    {
        if (Status != TestSuiteStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的测试集可以标记为就绪");

        if (!_testCases.Any())
            throw new InvalidOperationException("测试集必须至少包含一个测试用例才能标记为就绪");

        Status = TestSuiteStatus.Ready;
    }

    /// <summary>
    /// 将测试集重置为草稿状态。
    /// </summary>
    /// <exception cref="InvalidOperationException">如果测试集状态不是 Ready。</exception>
    public void ResetToDraft()
    {
        if (Status != TestSuiteStatus.Ready)
            throw new InvalidOperationException("只有就绪状态的测试集可以重置为草稿");

        Status = TestSuiteStatus.Draft;
    }

    /// <summary>
    /// 更新测试配置。
    /// </summary>
    /// <param name="newConfig">新的测试配置。</param>
    /// <exception cref="ArgumentNullException">如果配置为空。</exception>
    public void UpdateConfiguration(TestConfiguration newConfig)
    {
        Configuration = newConfig ?? throw new ArgumentNullException(nameof(newConfig));
    }

    /// <summary>
    /// 更新版本号。
    /// </summary>
    /// <param name="version">新版本号。</param>
    /// <exception cref="ArgumentException">如果版本号为空。</exception>
    public void UpdateVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("版本号不能为空", nameof(version));

        Version = version.Trim();
    }

    // 私有辅助方法
    private void CheckAndUpdateCompletion()
    {
        var pendingCases = _testCases.Where(tc =>
            tc.IsEnabled &&
            (tc.Status == TestCaseStatus.Pending || tc.Status == TestCaseStatus.Running)
        ).ToList();

        if (!pendingCases.Any())
        {
            Status = TestSuiteStatus.Completed;
            ExecutionEndTime = DateTime.UtcNow;
            UpdateAverageExecutionTime();
        }
    }

    private void UpdateAverageExecutionTime()
    {
        var executedCases = _testCases
            .Where(tc => tc.ExecutionDuration.HasValue)
            .ToList();

        if (executedCases.Any())
        {
            var totalDuration = TimeSpan.FromTicks(
                executedCases.Sum(tc => tc.ExecutionDuration!.Value.Ticks)
            );

            AverageExecutionTime = TimeSpan.FromTicks(totalDuration.Ticks / executedCases.Count);
        }
    }

    // 私有setter方法
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("名称不能为空", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("名称长度不能超过100个字符", nameof(name));

        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
    }

    private void SetProjectKey(string projectKey)
    {
        if (string.IsNullOrWhiteSpace(projectKey))
            throw new ArgumentException("项目标识不能为空", nameof(projectKey));

        if (projectKey.Length > 50)
            throw new ArgumentException("项目标识长度不能超过50个字符", nameof(projectKey));

        ProjectKey = projectKey.Trim().ToUpperInvariant();
    }
}
