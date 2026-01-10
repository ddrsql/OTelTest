using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public interface ITestSuiteDapperRepository : IRepository<TestSuite, Guid>
{
    // 可以定义特定于TestSuite的查询方法，例如根据名称或状态查询
    Task<TestSuite> GetWithCasesAsync(Guid id); // 明确加载子实体
    Task<List<TestSuite>> GetByStatusAsync(TestSuiteStatus status);
}
