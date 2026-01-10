using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace VoloAbp.OTel.TestSuites.Aggregates;

/// <summary>
/// 值对象：测试优先级
/// </summary>
public class TestPriority : ValueObject
{
    public static readonly TestPriority Low = new(1, "低");
    public static readonly TestPriority Medium = new(2, "中");
    public static readonly TestPriority High = new(3, "高");
    public static readonly TestPriority Critical = new(4, "紧急");

    public int Value { get; private set; }
    public string DisplayName { get; private set; }

    private TestPriority()
    {
        // Required by EF Core
    }

    private TestPriority(int value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }

    public static TestPriority FromValue(int value)
    {
        return value switch
        {
            1 => Low,
            2 => Medium,
            3 => High,
            4 => Critical,
            _ => throw new ArgumentException($"无效的优先级值: {value}")
        };
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
