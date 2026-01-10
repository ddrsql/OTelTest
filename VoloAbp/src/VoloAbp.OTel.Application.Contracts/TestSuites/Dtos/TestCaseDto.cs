using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestCaseDto : EntityDto<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Steps { get; set; }
    public string ExpectedResult { get; set; }
    public string ActualResult { get; set; }
    public bool IsEnabled { get; set; }
    public TestPriorityDto Priority { get; set; }
    public TestCaseStatus Status { get; set; }
    public DateTime? LastRunTime { get; set; }
    public string ExecutionDuration { get; set; }
    public string ErrorMessage { get; set; }
}
