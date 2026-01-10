using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Datas;

public class TestSuiteReport
{
    public Guid TestSuiteId { get; set; }
    public string TestSuiteName { get; set; }
    public int TotalTestCases { get; set; }
    public int PassedTestCases { get; set; }
    public int FailedTestCases { get; set; }
    public int SkippedTestCases { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    public List<TestCaseReport> TestCaseReports { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
