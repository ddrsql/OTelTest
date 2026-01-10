using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestSuiteStatisticsDto
{
    public int TotalTestSuites { get; set; }
    public int DraftCount { get; set; }
    public int ReadyCount { get; set; }
    public int RunningCount { get; set; }
    public int CompletedCount { get; set; }
    public int ArchivedCount { get; set; }
    public int TotalTestCases { get; set; }
    public double AverageSuccessRate { get; set; }
    public DateTime LastUpdated { get; set; }
}
