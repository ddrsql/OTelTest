using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VoloAbp.OTel.TestSuites;
using VoloAbp.OTel.TestSuites.Dtos;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.Controllers;

[RemoteService]
[Route("api/test-suites")]
public class TestSuiteController : OTelController, ITestSuiteAppService
{
    private readonly ITestSuiteAppService _testSuiteAppService;

    public TestSuiteController(ITestSuiteAppService testSuiteAppService)
    {
        _testSuiteAppService = testSuiteAppService;
    }

    [HttpPost]
    public Task<TestSuiteDto> CreateAsync(CreateUpdateTestSuiteDto input)
    {
        return _testSuiteAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _testSuiteAppService.DeleteAsync(id);
    }

    [HttpGet("{id}")]
    public Task<TestSuiteDto> GetAsync(Guid id)
    {
        return _testSuiteAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<TestSuiteDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _testSuiteAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<TestSuiteDto> UpdateAsync(Guid id, CreateUpdateTestSuiteDto input)
    {
        return _testSuiteAppService.UpdateAsync(id, input);
    }

    [HttpGet("project/{projectKey}")]
    public Task<PagedResultDto<TestSuiteDto>> GetByProjectAsync(string projectKey, PagedAndSortedResultRequestDto input)
    {
        return _testSuiteAppService.GetByProjectAsync(projectKey, input);
    }

    [HttpGet("status/{status}")]
    public Task<PagedResultDto<TestSuiteDto>> GetByStatusAsync(TestSuiteStatus status, PagedAndSortedResultRequestDto input)
    {
        return _testSuiteAppService.GetByStatusAsync(status, input);
    }

    [HttpPost("{testSuiteId}/test-cases")]
    public Task<TestCaseDto> AddTestCaseAsync(Guid testSuiteId, CreateUpdateTestCaseDto input)
    {
        return _testSuiteAppService.AddTestCaseAsync(testSuiteId, input);
    }

    [HttpPut("{testSuiteId}/test-cases/{testCaseId}")]
    public Task<TestCaseDto> UpdateTestCaseAsync(Guid testSuiteId, Guid testCaseId, UpdateTestCaseDto input)
    {
        return _testSuiteAppService.UpdateTestCaseAsync(testSuiteId, testCaseId, input);
    }

    [HttpDelete("{testSuiteId}/test-cases/{testCaseId}")]
    public Task DeleteTestCaseAsync(Guid testSuiteId, Guid testCaseId)
    {
        return _testSuiteAppService.DeleteTestCaseAsync(testSuiteId, testCaseId);
    }

    [HttpGet("{testSuiteId}/test-cases")]
    public Task<List<TestCaseDto>> GetTestCasesAsync(Guid testSuiteId)
    {
        return _testSuiteAppService.GetTestCasesAsync(testSuiteId);
    }

    [HttpGet("{testSuiteId}/test-cases/{testCaseId}")]
    public Task<TestCaseDto> GetTestCaseAsync(Guid testSuiteId, Guid testCaseId)
    {
        return _testSuiteAppService.GetTestCaseAsync(testSuiteId, testCaseId);
    }

    [HttpPost("{id}/execute")]
    public Task ExecuteTestSuiteAsync(Guid id)
    {
        return _testSuiteAppService.ExecuteTestSuiteAsync(id);
    }

    [HttpPost("{testSuiteId}/test-cases/{testCaseId}/record-result")]
    public Task RecordTestCaseResultAsync(Guid testSuiteId, Guid testCaseId, RecordTestCaseResultDto input)
    {
        return _testSuiteAppService.RecordTestCaseResultAsync(testSuiteId, testCaseId, input);
    }

    [HttpPost("{id}/complete")]
    public Task CompleteTestSuiteExecutionAsync(Guid id)
    {
        return _testSuiteAppService.CompleteTestSuiteExecutionAsync(id);
    }

    [HttpPost("{id}/fail")]
    public Task FailTestSuiteExecutionAsync(Guid id, [FromQuery] string reason)
    {
        return _testSuiteAppService.FailTestSuiteExecutionAsync(id, reason);
    }

    [HttpPost("{id}/mark-ready")]
    public Task MarkAsReadyAsync(Guid id)
    {
        return _testSuiteAppService.MarkAsReadyAsync(id);
    }

    [HttpPost("{id}/archive")]
    public Task ArchiveAsync(Guid id)
    {
        return _testSuiteAppService.ArchiveAsync(id);
    }

    [HttpPost("{id}/clone")]
    public Task CloneAsync(Guid id, CloneTestSuiteDto input)
    {
        return _testSuiteAppService.CloneAsync(id, input);
    }

    [HttpGet("statistics")]
    public Task<TestSuiteStatisticsDto> GetStatisticsAsync([FromQuery] string projectKey = null)
    {
        return _testSuiteAppService.GetStatisticsAsync(projectKey);
    }

    [HttpGet("{id}/report")]
    public Task<TestSuiteReportDto> GenerateReportAsync(Guid id)
    {
        return _testSuiteAppService.GenerateReportAsync(id);
    }

    [HttpGet("{id}/latest-report")]
    public Task<TestSuiteReportDto> GetLatestReportAsync(Guid id)
    {
        return _testSuiteAppService.GetLatestReportAsync(id);
    }

    [HttpPost("{id}/import-test-cases")]
    public Task ImportTestCasesAsync(Guid id, [FromBody] List<ImportTestCaseDto> inputs)
    {
        return _testSuiteAppService.ImportTestCasesAsync(id, inputs);
    }

    [HttpGet("{id}/export")]
    public async Task<IActionResult> ExportTestSuiteAsync(Guid id, [FromQuery] string format = "json")
    {
        var data = await _testSuiteAppService.ExportTestSuiteAsync(id, format);
        var fileName = $"testsuite-{id}.{format}";

        return File(data, "application/octet-stream", fileName);
    }

    // 显式实现接口，以满足编译要求，但实际调用走上面的方法
    async Task<byte[]> ITestSuiteAppService.ExportTestSuiteAsync(Guid id, string format)
    {
        return await _testSuiteAppService.ExportTestSuiteAsync(id, format);
    }
}