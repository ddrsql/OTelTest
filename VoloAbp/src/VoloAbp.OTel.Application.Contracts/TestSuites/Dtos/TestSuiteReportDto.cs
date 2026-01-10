using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestSuiteReportDto
{
    public Guid TestSuiteId { get; set; }
    public string TestSuiteName { get; set; }
    public int TotalTestCases { get; set; }
    public int PassedTestCases { get; set; }
    public int FailedTestCases { get; set; }
    public int SkippedTestCases { get; set; }
    public double SuccessRate { get; set; }
    public string TotalExecutionTime { get; set; }
    public List<TestCaseReportDto> TestCaseReports { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
