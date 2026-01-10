using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace VoloAbp.OTel.TestSuites.Aggregates;

/// <summary>
/// 值对象：测试配置
/// </summary>
public class TestConfiguration : ValueObject
{
    public int TimeoutInSeconds { get; private set; }
    public int MaxRetryCount { get; private set; }
    public bool EnableParallelExecution { get; private set; }
    public string Environment { get; private set; }

    public TestConfiguration(
        int timeoutInSeconds,
        int maxRetryCount,
        bool enableParallelExecution = false,
        string environment = "Development")
    {
        if (timeoutInSeconds <= 0)
            throw new ArgumentException("超时时间必须大于0", nameof(timeoutInSeconds));

        if (maxRetryCount < 0)
            throw new ArgumentException("最大重试次数不能为负数", nameof(maxRetryCount));

        if (string.IsNullOrWhiteSpace(environment))
            throw new ArgumentException("环境不能为空", nameof(environment));

        TimeoutInSeconds = timeoutInSeconds;
        MaxRetryCount = maxRetryCount;
        EnableParallelExecution = enableParallelExecution;
        Environment = environment.Trim();
    }

    // 值对象相等性比较
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return TimeoutInSeconds;
        yield return MaxRetryCount;
        yield return EnableParallelExecution;
        yield return Environment;
    }

    // 更新配置的方法
    public TestConfiguration WithTimeout(int timeoutInSeconds)
    {
        return new TestConfiguration(timeoutInSeconds, MaxRetryCount, EnableParallelExecution, Environment);
    }

    public TestConfiguration WithRetryCount(int maxRetryCount)
    {
        return new TestConfiguration(TimeoutInSeconds, maxRetryCount, EnableParallelExecution, Environment);
    }
}
