using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class CloneTestSuiteDto
{
    [Required]
    [StringLength(100)]
    public string NewName { get; set; }

    [StringLength(20)]
    public string NewVersion { get; set; }

    public bool CopyTestCases { get; set; } = true;
}
