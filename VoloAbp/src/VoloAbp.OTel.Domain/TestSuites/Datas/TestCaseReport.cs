using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Datas;

public class TestCaseReport
{
    public Guid TestCaseId { get; set; }
    public string Title { get; set; }
    public TestCaseStatus Status { get; set; }
    public TimeSpan ExecutionDuration { get; set; }
    public string ErrorMessage { get; set; }
}
