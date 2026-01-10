using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestCaseReportDto
{
    public Guid TestCaseId { get; set; }
    public string Title { get; set; }
    public TestCaseStatus Status { get; set; }
    public string ExecutionDuration { get; set; }
    public string ErrorMessage { get; set; }
}