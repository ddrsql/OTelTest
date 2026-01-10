using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using VoloAbp.OTel.TestSuites.Dtos;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public interface ITestSuiteAppService :
        ICrudAppService<
            TestSuiteDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateTestSuiteDto
        >
{
    // 自定义查询方法
    Task<PagedResultDto<TestSuiteDto>> GetByProjectAsync(string projectKey, PagedAndSortedResultRequestDto input);
    Task<PagedResultDto<TestSuiteDto>> GetByStatusAsync(TestSuiteStatus status, PagedAndSortedResultRequestDto input);

    // 测试用例管理
    Task<TestCaseDto> AddTestCaseAsync(Guid testSuiteId, CreateUpdateTestCaseDto input);
    Task<TestCaseDto> UpdateTestCaseAsync(Guid testSuiteId, Guid testCaseId, UpdateTestCaseDto input);
    Task DeleteTestCaseAsync(Guid testSuiteId, Guid testCaseId);
    Task<List<TestCaseDto>> GetTestCasesAsync(Guid testSuiteId);
    Task<TestCaseDto> GetTestCaseAsync(Guid testSuiteId, Guid testCaseId);

    // 测试执行
    Task ExecuteTestSuiteAsync(Guid id);
    Task RecordTestCaseResultAsync(Guid testSuiteId, Guid testCaseId, RecordTestCaseResultDto input);
    Task CompleteTestSuiteExecutionAsync(Guid id);
    Task FailTestSuiteExecutionAsync(Guid id, string reason);

    // 状态管理
    Task MarkAsReadyAsync(Guid id);
    Task ArchiveAsync(Guid id);
    Task CloneAsync(Guid id, CloneTestSuiteDto input);

    // 统计和报告
    Task<TestSuiteStatisticsDto> GetStatisticsAsync(string projectKey = null);
    Task<TestSuiteReportDto> GenerateReportAsync(Guid id);
    Task<TestSuiteReportDto> GetLatestReportAsync(Guid id);

    // 导入导出
    Task ImportTestCasesAsync(Guid id, List<ImportTestCaseDto> inputs);
    Task<byte[]> ExportTestSuiteAsync(Guid id, string format = "json");
}