using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.Tests.Dtos;

/// <summary>
/// Application 层可以看见 Domain 层；但 Domain 层必须对 Application 层（及其 DTO）完全“失明”
/// 传递具体参数（最推荐）
/// application层查询出实体Mapper后 -> 传递实体（Entity）
/// TestCaseImportInData = 定义领域模型（Domain Model/Value Object）
/// </summary>
public class TestCaseImportInModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Steps { get; set; }
    public string ExpectedResult { get; set; }
    public int Priority { get; set; } = 2;
}
