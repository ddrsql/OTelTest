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
    /// 复制测试集
    /// </summary>
    Task<TestSuite> CloneTestSuiteAsync(Guid sourceTestSuiteId, string newName, string newVersion = null);

    /// <summary>
    /// 批量导入测试用例
    /// </summary>
    Task ImportTestCasesAsync(Guid testSuiteId, List<TestCaseImportInModel> testCaseImports);

    /// <summary>
    /// 生成测试报告
    /// </summary>
    Task<TestSuiteReport> GenerateTestReportAsync(Guid testSuiteId);

    /// <summary>
    /// 验证测试集配置
    /// </summary>
    Task<bool> ValidateTestSuiteConfigurationAsync(Guid testSuiteId);
}
