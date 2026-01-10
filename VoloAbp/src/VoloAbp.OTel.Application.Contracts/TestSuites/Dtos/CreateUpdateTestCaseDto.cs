using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class CreateUpdateTestCaseDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    public string Steps { get; set; }

    [Required]
    public string ExpectedResult { get; set; }

    [Range(1, 4)]
    public int Priority { get; set; } = 2;
}
