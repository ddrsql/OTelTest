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

    /// <summary>
    /// 复制测试集。
    /// </summary>
    /// <param name="sourceTestSuiteId">源测试集ID。</param>
    /// <param name="newName">新测试集名称。</param>
    /// <param name="newVersion">新测试集版本（可选）。</param>
    /// <returns>创建的克隆测试集。</returns>
    /// <exception cref="ArgumentException">如果源测试集未找到或新名称为空。</exception>
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

    /// <summary>
    /// 批量导入测试用例。
    /// </summary>
    /// <param name="testSuiteId">目标测试集ID。</param>
    /// <param name="testCaseImports">要导入的测试用例列表。</param>
    /// <returns>导入结果。</returns>
    /// <exception cref="ArgumentException">如果测试集未找到。</exception>
    /// <exception cref="InvalidOperationException">如果测试集已归档。</exception>
    public async Task<TestCaseImportResult> ImportTestCasesAsync(Guid testSuiteId, List<TestCaseImportInModel> testCaseImports)
    {
        var testSuite = await _testSuiteRepository.FindAsync(testSuiteId);
        if (testSuite == null)
            throw new ArgumentException($"未找到ID为 {testSuiteId} 的测试集");

        if (testSuite.Status == TestSuiteStatus.Archived)
            throw new InvalidOperationException("已归档的测试集不能导入测试用例");

        var result = new TestCaseImportResult
        {
            TotalCount = testCaseImports.Count
        };

        var index = 0;
        foreach (var import in testCaseImports)
        {
            index++;
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

                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.FailedImports.Add(new FailedImport
                {
                    Index = index,
                    Title = import.Title,
                    ErrorMessage = ex.Message
                });
                
                // 记录警告日志
                Logger.LogWarning($"导入测试用例失败 (Row {index}): {ex.Message}");
            }
        }

        await _testSuiteRepository.UpdateAsync(testSuite);
        return result;
    }

    /// <summary>
    /// 生成测试报告。
    /// </summary>
    /// <param name="testSuiteId">测试集ID。</param>
    /// <returns>生成的测试报告。</returns>
    /// <exception cref="ArgumentException">如果测试集未找到。</exception>
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
                executedCases.Sum(tc => tc.ExecutionDuration!.Value.Ticks)
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

    /// <summary>
    /// 验证测试集配置。
    /// </summary>
    /// <param name="testSuiteId">测试集ID。</param>
    /// <returns>如果配置有效且可执行则返回 true，否则返回 false。</returns>
    /// <exception cref="ArgumentException">如果测试集未找到。</exception>
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
