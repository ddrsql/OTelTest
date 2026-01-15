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
    /// <summary>
    /// 测试用例标题
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// 测试用例描述
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 测试步骤
    /// </summary>
    public string Steps { get; private set; }

    /// <summary>
    /// 预期结果
    /// </summary>
    public string ExpectedResult { get; private set; }

    /// <summary>
    /// 实际结果
    /// </summary>
    public string ActualResult { get; private set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; private set; } = true;

    /// <summary>
    /// 优先级
    /// </summary>
    public TestPriority Priority { get; private set; } = TestPriority.Medium;

    /// <summary>
    /// 当前状态
    /// </summary>
    public TestCaseStatus Status { get; private set; } = TestCaseStatus.NotRun;

    /// <summary>
    /// 最后运行时间
    /// </summary>
    public DateTime? LastRunTime { get; private set; }

    /// <summary>
    /// 执行耗时
    /// </summary>
    public TimeSpan? ExecutionDuration { get; private set; }

    /// <summary>
    /// 错误信息
    /// </summary>
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
    /// <summary>
    /// 更新测试用例的详细信息。
    /// </summary>
    /// <param name="title">新标题。</param>
    /// <param name="description">新描述。</param>
    /// <param name="steps">新步骤。</param>
    /// <param name="expectedResult">新预期结果。</param>
    /// <param name="priority">新优先级（可选）。</param>
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

    /// <summary>
    /// 记录执行结果。
    /// </summary>
    /// <param name="duration">执行耗时。</param>
    /// <param name="errorMessage">错误信息。如果为空，则视为通过。</param>
    public void RecordExecutionResult(TimeSpan? duration = null, string errorMessage = null)
    {
        LastRunTime = DateTime.UtcNow;
        ExecutionDuration = duration;
        ErrorMessage = errorMessage;

        Status = string.IsNullOrEmpty(errorMessage)
            ? TestCaseStatus.Passed
            : TestCaseStatus.Failed;
    }

    /// <summary>
    /// 标记为待执行状态，重置之前的执行结果。
    /// </summary>
    public void MarkAsPending()
    {
        Status = TestCaseStatus.Pending;
        LastRunTime = null;
        ExecutionDuration = null;
        ErrorMessage = null;
    }

    /// <summary>
    /// 禁用测试用例。
    /// </summary>
    public void Disable() => IsEnabled = false;

    /// <summary>
    /// 启用测试用例。
    /// </summary>
    public void Enable() => IsEnabled = true;

    /// <summary>
    /// 更新实际执行结果描述。
    /// </summary>
    /// <param name="actualResult">实际结果描述。</param>
    public void UpdateActualResult(string actualResult)
    {
        ActualResult = actualResult?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 检查测试用例是否已被执行。
    /// </summary>
    /// <returns>如果已执行（有最后运行时间）则返回 true。</returns>
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
