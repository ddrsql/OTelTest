using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestSuiteDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProjectKey { get; set; }
    public string Version { get; set; }
    public TestSuiteStatus Status { get; set; }
    public TestConfigurationDto Configuration { get; set; }
    public int TotalTestCases { get; set; }
    public int PassedTestCases { get; set; }
    public int FailedTestCases { get; set; }
    public double SuccessRate { get; set; }
    public DateTime? LastExecutionTime { get; set; }
    public string AverageExecutionTime { get; set; }
    public List<TestCaseDto> TestCases { get; set; } = new();
}
