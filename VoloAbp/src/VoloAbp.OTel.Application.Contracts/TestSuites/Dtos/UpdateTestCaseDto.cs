using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class UpdateTestCaseDto
{
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    public string Steps { get; set; }

    public string ExpectedResult { get; set; }

    [Range(1, 4)]
    public int? Priority { get; set; }

    public bool? IsEnabled { get; set; }
}
