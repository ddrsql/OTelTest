using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoloAbp.OTel.TestSuites.Enums;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class RecordTestCaseResultDto
{
    [Required]
    public TestCaseStatus Status { get; set; }

    [Range(0, int.MaxValue)]
    public int? DurationInSeconds { get; set; }

    [StringLength(2000)]
    public string ErrorMessage { get; set; }

    [StringLength(5000)]
    public string ActualResult { get; set; }
}
