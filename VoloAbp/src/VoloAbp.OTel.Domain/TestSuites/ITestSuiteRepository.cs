using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public interface ITestSuiteRepository : IRepository<TestSuite, Guid>
{
    /// <summary>
    /// 根据名称查找测试集
    /// </summary>
    Task<TestSuite> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据项目标识获取测试集列表
    /// </summary>
    Task<List<TestSuite>> GetByProjectKeyAsync(string projectKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据状态获取测试集列表
    /// </summary>
    Task<List<TestSuite>> GetByStatusAsync(TestSuiteStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定时间段内的测试集
    /// </summary>
    Task<List<TestSuite>> GetByTimeRangeAsync(
        DateTime? startTime,
        DateTime? endTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取包含测试用例的测试集
    /// </summary>
    Task<TestSuite> GetWithTestCasesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取测试集统计信息
    /// </summary>
    Task<TestSuiteStatistics> GetStatisticsAsync(
        string projectKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新测试集状态
    /// </summary>
    Task<int> BulkUpdateStatusAsync(
        List<Guid> testSuiteIds,
        TestSuiteStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取最近执行的测试集
    /// </summary>
    Task<List<TestSuite>> GetRecentlyExecutedAsync(
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查测试集是否存在
    /// </summary>
    Task<bool> ExistsByNameAndProjectAsync(
        string name,
        string projectKey,
        CancellationToken cancellationToken = default);
}
