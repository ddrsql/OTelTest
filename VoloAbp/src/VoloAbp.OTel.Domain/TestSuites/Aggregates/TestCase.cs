using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Aggregates;


/// <summary>
/// 实体：测试用例
/// </summary>
public class TestCase : Entity<Guid>
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Steps { get; private set; }
    public string ExpectedResult { get; private set; }
    public string ActualResult { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public TestPriority Priority { get; private set; } = TestPriority.Medium;
    public TestCaseStatus Status { get; private set; } = TestCaseStatus.NotRun;
    public DateTime? LastRunTime { get; private set; }
    public TimeSpan? ExecutionDuration { get; private set; }
    public string ErrorMessage { get; private set; }

    // 供 EF Core 使用
    private TestCase() { }

    // 仅供EF Core或聚合根内部使用
    internal TestCase(
        Guid id,
        string title,
        string description,
        string steps,
        string expectedResult,
        TestPriority priority = null)
    {
        Id = id;
        SetTitle(title);
        SetDescription(description);
        SetSteps(steps);
        SetExpectedResult(expectedResult);
        Priority = priority ?? TestPriority.Medium;
    }

    // 业务方法
    public void UpdateDetails(
        string title,
        string description,
        string steps,
        string expectedResult,
        TestPriority priority = null)
    {
        SetTitle(title);
        SetDescription(description);
        SetSteps(steps);
        SetExpectedResult(expectedResult);

        if (priority != null)
            Priority = priority;
    }

    public void RecordExecutionResult(TimeSpan? duration = null, string errorMessage = null)
    {
        LastRunTime = DateTime.UtcNow;
        ExecutionDuration = duration;
        ErrorMessage = errorMessage;

        Status = string.IsNullOrEmpty(errorMessage)
            ? TestCaseStatus.Passed
            : TestCaseStatus.Failed;
    }

    public void MarkAsPending()
    {
        Status = TestCaseStatus.Pending;
        LastRunTime = null;
        ExecutionDuration = null;
        ErrorMessage = null;
    }

    public void Disable() => IsEnabled = false;
    public void Enable() => IsEnabled = true;

    public void UpdateActualResult(string actualResult)
    {
        ActualResult = actualResult?.Trim() ?? string.Empty;
    }

    public bool HasBeenExecuted() => LastRunTime.HasValue;

    // 私有setter方法
    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("测试用例标题不能为空", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("测试用例标题长度不能超过200个字符", nameof(title));

        Title = title.Trim();
    }

    private void SetDescription(string description)
    {
        Description = description?.Trim() ?? string.Empty;
    }

    private void SetSteps(string steps)
    {
        if (string.IsNullOrWhiteSpace(steps))
            throw new ArgumentException("测试步骤不能为空", nameof(steps));

        Steps = steps.Trim();
    }

    private void SetExpectedResult(string expectedResult)
    {
        if (string.IsNullOrWhiteSpace(expectedResult))
            throw new ArgumentException("预期结果不能为空", nameof(expectedResult));

        ExpectedResult = expectedResult.Trim();
    }
}
