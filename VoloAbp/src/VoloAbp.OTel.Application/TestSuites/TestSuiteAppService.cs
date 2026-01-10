using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using VoloAbp.OTel.Tests.Dtos;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;
using VoloAbp.OTel.TestSuites.Dtos;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public class TestSuiteAppService :
    CrudAppService<
        TestSuite,
        TestSuiteDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateTestSuiteDto
    >,
    ITestSuiteAppService, IOTelActivityEnabled
{
    private readonly ITestSuiteRepository _testSuiteRepository;
    private readonly ITestSuiteManager _testSuiteManager;
    private readonly IGuidGenerator _guidGenerator;

    public TestSuiteAppService(
        IRepository<TestSuite, Guid> repository,
        ITestSuiteRepository testSuiteRepository,
        ITestSuiteManager testSuiteManager,
        IGuidGenerator guidGenerator)
        : base(repository)
    {
        _testSuiteRepository = testSuiteRepository;
        _testSuiteManager = testSuiteManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<PagedResultDto<TestSuiteDto>> GetByProjectAsync(string projectKey, PagedAndSortedResultRequestDto input)
    {
        var testSuites = await _testSuiteRepository.GetByProjectKeyAsync(projectKey);

        var totalCount = testSuites.Count;
        var items = ObjectMapper.Map<List<TestSuite>, List<TestSuiteDto>>(testSuites);

        return new PagedResultDto<TestSuiteDto>(totalCount, items);
    }

    public async Task<PagedResultDto<TestSuiteDto>> GetByStatusAsync(TestSuiteStatus status, PagedAndSortedResultRequestDto input)
    {
        var testSuites = await _testSuiteRepository.GetByStatusAsync(status);

        var totalCount = testSuites.Count;
        var items = ObjectMapper.Map<List<TestSuite>, List<TestSuiteDto>>(testSuites);

        return new PagedResultDto<TestSuiteDto>(totalCount, items);
    }

    public async Task<TestCaseDto> AddTestCaseAsync(Guid testSuiteId, CreateUpdateTestCaseDto input)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);

        var priority = TestPriority.FromValue(input.Priority);

        testSuite.AddTestCase(
            input.Title,
            input.Description ?? string.Empty,
            input.Steps,
            input.ExpectedResult,
            priority
        );

        await _testSuiteRepository.UpdateAsync(testSuite);

        var addedTestCase = testSuite.TestCases.Last();
        return ObjectMapper.Map<TestCase, TestCaseDto>(addedTestCase);
    }

    public async Task<TestCaseDto> UpdateTestCaseAsync(Guid testSuiteId, Guid testCaseId, UpdateTestCaseDto input)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);

        TestPriority priority = null;
        if (input.Priority.HasValue)
        {
            priority = TestPriority.FromValue(input.Priority.Value);
        }

        testSuite.UpdateTestCase(
            testCaseId,
            input.Title,
            input.Description,
            input.Steps,
            input.ExpectedResult,
            priority
        );

        if (input.IsEnabled.HasValue)
        {
            if (input.IsEnabled.Value)
            {
                testSuite.GetTestCase(testCaseId).Enable();
            }
            else
            {
                testSuite.GetTestCase(testCaseId).Disable();
            }
        }

        await _testSuiteRepository.UpdateAsync(testSuite);

        var updatedTestCase = testSuite.GetTestCase(testCaseId);
        return ObjectMapper.Map<TestCase, TestCaseDto>(updatedTestCase);
    }

    public async Task DeleteTestCaseAsync(Guid testSuiteId, Guid testCaseId)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);
        testSuite.RemoveTestCase(testCaseId);
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task<List<TestCaseDto>> GetTestCasesAsync(Guid testSuiteId)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);
        return ObjectMapper.Map<List<TestCase>, List<TestCaseDto>>(testSuite.TestCases.ToList());
    }

    public async Task<TestCaseDto> GetTestCaseAsync(Guid testSuiteId, Guid testCaseId)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);
        var testCase = testSuite.GetTestCase(testCaseId);
        return ObjectMapper.Map<TestCase, TestCaseDto>(testCase);
    }

    public async Task ExecuteTestSuiteAsync(Guid id)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        testSuite.Execute();
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task RecordTestCaseResultAsync(Guid testSuiteId, Guid testCaseId, RecordTestCaseResultDto input)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(testSuiteId);

        TimeSpan? duration = input.DurationInSeconds.HasValue
            ? TimeSpan.FromSeconds(input.DurationInSeconds.Value)
            : null;

        testSuite.RecordTestCaseResult(testCaseId, input.Status, duration, input.ErrorMessage, input.ActualResult);
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task CompleteTestSuiteExecutionAsync(Guid id)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        testSuite.CompleteExecution();
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task FailTestSuiteExecutionAsync(Guid id, string reason)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        testSuite.FailExecution(reason);
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task MarkAsReadyAsync(Guid id)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        testSuite.MarkAsReady();
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task ArchiveAsync(Guid id)
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        testSuite.Archive();
        await _testSuiteRepository.UpdateAsync(testSuite);
    }

    public async Task CloneAsync(Guid id, CloneTestSuiteDto input)
    {
        var clonedTestSuite = await _testSuiteManager.CloneTestSuiteAsync(id, input.NewName, input.NewVersion);
        await _testSuiteRepository.InsertAsync(clonedTestSuite);
    }

    public async Task<TestSuiteStatisticsDto> GetStatisticsAsync(string projectKey = null)
    {
        var statistics = await _testSuiteRepository.GetStatisticsAsync(projectKey);
        return ObjectMapper.Map<TestSuiteStatistics, TestSuiteStatisticsDto>(statistics);
    }

    public async Task<TestSuiteReportDto> GenerateReportAsync(Guid id)
    {
        var report = await _testSuiteManager.GenerateTestReportAsync(id);
        return ObjectMapper.Map<TestSuiteReport, TestSuiteReportDto>(report);
    }

    public async Task<TestSuiteReportDto> GetLatestReportAsync(Guid id)
    {
        // 这里简化实现，实际应用中可能从数据库获取缓存的报告
        return await GenerateReportAsync(id);
    }

    public async Task ImportTestCasesAsync(Guid id, List<ImportTestCaseDto> inputs)
    {
        var testCaseImports = inputs.Select(input => new TestCaseImportInModel
        {
            Title = input.Title,
            Description = input.Description,
            Steps = input.Steps,
            ExpectedResult = input.ExpectedResult,
            Priority = input.Priority
        }).ToList();

        await _testSuiteManager.ImportTestCasesAsync(id, testCaseImports);
    }

    public async Task<byte[]> ExportTestSuiteAsync(Guid id, string format = "json")
    {
        var testSuite = await _testSuiteRepository.GetWithTestCasesAsync(id);
        var testSuiteDto = ObjectMapper.Map<TestSuite, TestSuiteDto>(testSuite);

        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(testSuiteDto, options);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
        else
        {
            throw new NotSupportedException($"不支持的导出格式: {format}");
        }
    }

    protected override async Task<TestSuite> GetEntityByIdAsync(Guid id)
    {
        return await _testSuiteRepository.GetWithTestCasesAsync(id);
    }
}
