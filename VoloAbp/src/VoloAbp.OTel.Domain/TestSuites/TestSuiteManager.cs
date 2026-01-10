using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using VoloAbp.OTel.Tests.Dtos;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public class TestSuiteManager : OTelDomainService, ITestSuiteManager
{
    private readonly IRepository<TestSuite, Guid> _testSuiteRepository;
    private readonly IGuidGenerator _guidGenerator;

    public TestSuiteManager(
        IRepository<TestSuite, Guid> testSuiteRepository,
        IGuidGenerator guidGenerator)
    {
        _testSuiteRepository = testSuiteRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task<TestSuite> CloneTestSuiteAsync(Guid sourceTestSuiteId, string newName, string newVersion = null)
    {
        var sourceTestSuite = await _testSuiteRepository.FindAsync(sourceTestSuiteId);
        if (sourceTestSuite == null)
            throw new ArgumentException($"未找到ID为 {sourceTestSuiteId} 的测试集");

        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("新测试集名称不能为空", nameof(newName));

        // 创建新的测试集
        var clonedTestSuite = new TestSuite(
            _guidGenerator.Create(),
            newName,
            $"{sourceTestSuite.Description} (克隆自 {sourceTestSuite.Name})",
            sourceTestSuite.ProjectKey,
            newVersion ?? $"克隆-{DateTime.UtcNow:yyyyMMdd}",
            sourceTestSuite.Configuration
        );

        // 复制所有测试用例
        foreach (var testCase in sourceTestSuite.TestCases)
        {
            clonedTestSuite.AddTestCase(
                $"{testCase.Title} (克隆)",
                testCase.Description,
                testCase.Steps,
                testCase.ExpectedResult,
                testCase.Priority
            );
        }

        return clonedTestSuite;
    }

    public async Task ImportTestCasesAsync(Guid testSuiteId, List<TestCaseImportInModel> testCaseImports)
    {
        var testSuite = await _testSuiteRepository.FindAsync(testSuiteId);
        if (testSuite == null)
            throw new ArgumentException($"未找到ID为 {testSuiteId} 的测试集");

        if (testSuite.Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能导入测试用例");

        foreach (var import in testCaseImports)
        {
            try
            {
                var priority = TestPriority.FromValue(import.Priority);

                testSuite.AddTestCase(
                    import.Title,
                    import.Description ?? string.Empty,
                    import.Steps,
                    import.ExpectedResult,
                    priority
                );
            }
            catch (Exception ex)
            {
                // 记录错误但继续处理其他用例
                Logger.LogWarning($"导入测试用例失败: {ex.Message}");
            }
        }

        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task<TestSuiteReport> GenerateTestReportAsync(Guid testSuiteId)
    {
        var testSuite = await _testSuiteRepository.FindAsync(testSuiteId);
        if (testSuite == null)
            throw new ArgumentException($"未找到ID为 {testSuiteId} 的测试集");

        var report = new TestSuiteReport
        {
            TestSuiteId = testSuite.Id,
            TestSuiteName = testSuite.Name,
            TotalTestCases = testSuite.TotalTestCases,
            PassedTestCases = testSuite.PassedTestCases,
            FailedTestCases = testSuite.FailedTestCases,
            SkippedTestCases = testSuite.TestCases.Count(tc => tc.Status == TestCaseStatus.Skipped),
            SuccessRate = testSuite.SuccessRate
        };

        // 计算总执行时间
        var executedCases = testSuite.TestCases
            .Where(tc => tc.ExecutionDuration.HasValue)
            .ToList();

        if (executedCases.Any())
        {
            report.TotalExecutionTime = TimeSpan.FromTicks(
                executedCases.Sum(tc => tc.ExecutionDuration.Value.Ticks)
            );
        }

        // 生成每个测试用例的报告
        foreach (var testCase in testSuite.TestCases)
        {
            report.TestCaseReports.Add(new TestCaseReport
            {
                TestCaseId = testCase.Id,
                Title = testCase.Title,
                Status = testCase.Status,
                ExecutionDuration = testCase.ExecutionDuration ?? TimeSpan.Zero,
                ErrorMessage = testCase.ErrorMessage
            });
        }

        return report;
    }

    public async Task<bool> ValidateTestSuiteConfigurationAsync(Guid testSuiteId)
    {
        var testSuite = await _testSuiteRepository.FindAsync(testSuiteId);
        if (testSuite == null)
            throw new ArgumentException($"未找到ID为 {testSuiteId} 的测试集");

        // 验证测试集是否有测试用例
        if (!testSuite.TestCases.Any())
            return false;

        // 验证是否有启用的测试用例
        if (!testSuite.TestCases.Any(tc => tc.IsEnabled))
            return false;

        // 验证测试集状态
        if (testSuite.Status == TestSuiteStatus.Archived)
            return false;

        return true;
    }
}
