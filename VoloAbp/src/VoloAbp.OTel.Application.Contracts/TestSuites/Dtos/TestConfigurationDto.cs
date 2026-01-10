using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoloAbp.OTel.TestSuites.Dtos;

public class TestConfigurationDto
{
    [Range(1, 3600)]
    public int TimeoutInSeconds { get; set; } = 30;

    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;

    public bool EnableParallelExecution { get; set; }

    [Required]
    [StringLength(50)]
    public string Environment { get; set; } = "Development";
}
