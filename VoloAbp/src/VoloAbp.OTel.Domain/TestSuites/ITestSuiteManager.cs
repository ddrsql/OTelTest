using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VoloAbp.OTel.Tests.Dtos;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;

namespace VoloAbp.OTel.TestSuites;

public interface ITestSuiteManager : IDomainService
{
    /// <summary>
    /// 复制测试集。
    /// </summary>
    /// <param name="sourceTestSuiteId">源测试集ID。</param>
    /// <param name="newName">新测试集名称。</param>
    /// <param name="newVersion">新测试集版本（可选）。</param>
    /// <returns>创建的克隆测试集。</returns>
    Task<TestSuite> CloneTestSuiteAsync(Guid sourceTestSuiteId, string newName, string newVersion = null);

    /// <summary>
    /// 批量导入测试用例。
    /// </summary>
    /// <param name="testSuiteId">目标测试集ID。</param>
    /// <param name="testCaseImports">要导入的测试用例列表。</param>
    /// <returns>导入结果。</returns>
    Task<TestCaseImportResult> ImportTestCasesAsync(Guid testSuiteId, List<TestCaseImportInModel> testCaseImports);

    /// <summary>
    /// 生成测试报告。
    /// </summary>
    /// <param name="testSuiteId">测试集ID。</param>
    /// <returns>生成的测试报告。</returns>
    Task<TestSuiteReport> GenerateTestReportAsync(Guid testSuiteId);

    /// <summary>
    /// 验证测试集配置。
    /// </summary>
    /// <param name="testSuiteId">测试集ID。</param>
    /// <returns>如果配置有效且可执行则返回 true，否则返回 false。</returns>
    Task<bool> ValidateTestSuiteConfigurationAsync(Guid testSuiteId);
}
