using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Enums;

public enum TestCaseStatus
{
    /// <summary>
    /// 未运行
    /// </summary>
    NotRun,
    /// <summary>
    /// 待定
    /// </summary>
    Pending,
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    /// <summary>
    /// 通过
    /// </summary>
    Passed,
    /// <summary>
    /// 失败
    /// </summary>
    Failed,
    /// <summary>
    /// 跳过
    /// </summary>
    Skipped
}
