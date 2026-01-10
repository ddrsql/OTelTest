using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VoloAbp.OTel.EntityFrameworkCore;
using VoloAbp.OTel.TestSuites.Aggregates;
using VoloAbp.OTel.TestSuites.Datas;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites;

public class EfCoreTestSuiteRepository : EfCoreRepository<OTelDbContext, TestSuite, Guid>, ITestSuiteRepository
{
    public EfCoreTestSuiteRepository(IDbContextProvider<OTelDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<TestSuite> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .FirstOrDefaultAsync(ts => ts.Name == name, cancellationToken);
    }

    public async Task<List<TestSuite>> GetByProjectKeyAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .Where(ts => ts.ProjectKey == projectKey)
            .OrderByDescending(ts => ts.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TestSuite>> GetByStatusAsync(TestSuiteStatus status, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .Where(ts => ts.Status == status)
            .OrderByDescending(ts => ts.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TestSuite>> GetByTimeRangeAsync(
        DateTime? startTime,
        DateTime? endTime,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .AsQueryable();

        if (startTime.HasValue)
        {
            query = query.Where(ts => ts.CreationTime >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            query = query.Where(ts => ts.CreationTime <= endTime.Value);
        }

        return await query
            .OrderByDescending(ts => ts.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<TestSuite> GetWithTestCasesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);
    }

    public async Task<TestSuiteStatistics> GetStatisticsAsync(
        string projectKey = null,
        CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(projectKey))
        {
            query = query.Where(ts => ts.ProjectKey == projectKey);
        }

        var testSuites = await query.ToListAsync(cancellationToken);

        var statistics = new TestSuiteStatistics
        {
            TotalTestSuites = testSuites.Count,
            DraftCount = testSuites.Count(ts => ts.Status == TestSuiteStatus.Draft),
            ReadyCount = testSuites.Count(ts => ts.Status == TestSuiteStatus.Ready),
            RunningCount = testSuites.Count(ts => ts.Status == TestSuiteStatus.Running),
            CompletedCount = testSuites.Count(ts => ts.Status == TestSuiteStatus.Completed),
            ArchivedCount = testSuites.Count(ts => ts.Status == TestSuiteStatus.Archived),
            TotalTestCases = testSuites.Sum(ts => ts.TotalTestCases)
        };

        var completedSuites = testSuites.Where(ts => ts.Status == TestSuiteStatus.Completed).ToList();
        if (completedSuites.Any())
        {
            statistics.AverageSuccessRate = completedSuites.Average(ts => ts.SuccessRate);
        }

        return statistics;
    }

    public async Task<int> BulkUpdateStatusAsync(
        List<Guid> testSuiteIds,
        TestSuiteStatus status,
        CancellationToken cancellationToken = default)
    {
        var testSuites = await (await GetDbSetAsync())
            .Where(ts => testSuiteIds.Contains(ts.Id))
            .ToListAsync(cancellationToken);

        foreach (var testSuite in testSuites)
        {
            // 根据状态执行相应的业务逻辑
            switch (status)
            {
                case TestSuiteStatus.Ready:
                    testSuite.MarkAsReady();
                    break;
                case TestSuiteStatus.Archived:
                    testSuite.Archive();
                    break;
                default:
                    // 其他状态可以直接设置
                    // 注意：这里省略了状态转换的验证，实际应用中应该添加
                    break;
            }
        }

        await UpdateManyAsync(testSuites, cancellationToken: cancellationToken);
        return testSuites.Count;
    }

    public async Task<List<TestSuite>> GetRecentlyExecutedAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Include(ts => ts.TestCases)
            .Where(ts => ts.LastExecutionTime.HasValue)
            .OrderByDescending(ts => ts.LastExecutionTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndProjectAsync(
        string name,
        string projectKey,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AnyAsync(ts => ts.Name == name && ts.ProjectKey == projectKey, cancellationToken);
    }

    // 重写DeleteAsync以处理级联删除
    public override async Task DeleteAsync(
        TestSuite entity,
        bool autoSave = false,
        CancellationToken cancellationToken = default)
    {
        // 如果需要级联删除测试用例，可以在这里实现
        // 或者配置DbContext中的OnModelCreating来处理
        await base.DeleteAsync(entity, autoSave, cancellationToken);
    }
}