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
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ProjectKey { get; private set; }
    public string Version { get; private set; }
    public TestConfiguration Configuration { get; private set; }
    public TestSuiteStatus Status { get; private set; } = TestSuiteStatus.Draft;
    public int TotalTestCases => _testCases.Count;
    public int PassedTestCases => _testCases.Count(tc => tc.Status == TestCaseStatus.Passed);
    public int FailedTestCases => _testCases.Count(tc => tc.Status == TestCaseStatus.Failed);
    public double SuccessRate => TotalTestCases > 0 ? (double)PassedTestCases / TotalTestCases * 100 : 0;
    public DateTime? LastExecutionTime { get; private set; }
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

    public void RemoveTestCase(Guid testCaseId)
    {
        if (Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能删除测试用例");

        var testCase = _testCases.FirstOrDefault(tc => tc.Id == testCaseId);
        if (testCase == null)
            throw new KeyNotFoundException($"未找到ID为 {testCaseId} 的测试用例");

        _testCases.Remove(testCase);
    }

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

        testCase.UpdateDetails(
            title ?? testCase.Title,
            description ?? testCase.Description,
            steps ?? testCase.Steps,
            expectedResult ?? testCase.ExpectedResult,
            priority ?? testCase.Priority
        );
    }

    public TestCase GetTestCase(Guid testCaseId)
    {
        return _testCases.FirstOrDefault(tc => tc.Id == testCaseId)
            ?? throw new KeyNotFoundException($"未找到ID为 {testCaseId} 的测试用例");
    }

    public void Execute()
    {
        if (Status != TestSuiteStatus.Ready && Status != TestSuiteStatus.Draft)
            throw new InvalidOperationException($"测试集状态为 '{Status}'，无法执行");

        var enabledCases = _testCases.Where(tc => tc.IsEnabled).ToList();
        if (!enabledCases.Any())
            throw new InvalidOperationException("没有启用的测试用例用于执行");

        Status = TestSuiteStatus.Running;
        LastExecutionTime = DateTime.UtcNow;

        // 将所有启用的测试用例标记为待执行
        foreach (var testCase in enabledCases)
        {
            testCase.MarkAsPending();
        }
    }

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

    public void CompleteExecution()
    {
        if (Status != TestSuiteStatus.Running)
            throw new InvalidOperationException("只有运行中的测试集可以标记为完成");

        Status = TestSuiteStatus.Completed;
        UpdateAverageExecutionTime();
    }

    public void FailExecution(string reason)
    {
        if (Status != TestSuiteStatus.Running)
            throw new InvalidOperationException("只有运行中的测试集可以标记为失败");

        Status = TestSuiteStatus.Completed;
        UpdateAverageExecutionTime();

        // 标记所有未完成的测试用例为失败
        foreach (var testCase in _testCases.Where(tc => tc.Status == TestCaseStatus.Pending))
        {
            testCase.RecordExecutionResult(null, $"测试集执行失败: {reason}");
        }
    }

    public void Archive()
    {
        if (Status != TestSuiteStatus.Completed && Status != TestSuiteStatus.Ready)
            throw new InvalidOperationException("只有已完成或就绪的测试集可以被归档");

        Status = TestSuiteStatus.Archived;
    }

    public void MarkAsReady()
    {
        if (Status != TestSuiteStatus.Draft)
            throw new InvalidOperationException("只有草稿状态的测试集可以标记为就绪");

        if (!_testCases.Any())
            throw new InvalidOperationException("测试集必须至少包含一个测试用例才能标记为就绪");

        Status = TestSuiteStatus.Ready;
    }

    public void UpdateConfiguration(TestConfiguration newConfig)
    {
        Configuration = newConfig ?? throw new ArgumentNullException(nameof(newConfig));
    }

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
                executedCases.Sum(tc => tc.ExecutionDuration.Value.Ticks)
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
