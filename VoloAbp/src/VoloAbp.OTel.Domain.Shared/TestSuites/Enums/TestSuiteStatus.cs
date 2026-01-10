using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Enums;

public enum TestSuiteStatus
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft,

    /// <summary>
    /// 就绪
    /// </summary>
    Ready,

    /// <summary>
    /// 执行中
    /// </summary>
    Running,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed,

    /// <summary>
    /// 已归档
    /// </summary>
    Archived
}
