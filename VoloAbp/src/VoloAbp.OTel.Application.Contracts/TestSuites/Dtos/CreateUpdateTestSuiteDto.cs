using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class CreateUpdateTestSuiteDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [StringLength(50)]
    public string ProjectKey { get; set; }

    [StringLength(20)]
    public string Version { get; set; } = "1.0.0";

    public TestConfigurationDto Configuration { get; set; }
}
