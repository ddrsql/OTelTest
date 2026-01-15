using System.Collections.Generic;

namespace VoloAbp.OTel.TestSuites.Datas;

/// <summary>
/// 测试用例导入结果
/// </summary>
public class TestCaseImportResult
{
    /// <summary>
    /// 总导入数量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功数量
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败数量
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// 失败详情列表
    /// </summary>
    public List<FailedImport> FailedImports { get; set; } = new();
}

/// <summary>
/// 导入失败详情
/// </summary>
public class FailedImport
{
    /// <summary>
    /// 数据索引（行号或顺序号）
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 尝试导入的标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; }
}
