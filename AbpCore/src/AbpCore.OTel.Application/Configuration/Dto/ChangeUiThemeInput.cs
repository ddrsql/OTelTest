using System.ComponentModel.DataAnnotations;

namespace AbpCore.OTel.Configuration.Dto;

public class ChangeUiThemeInput
{
    [Required]
    [StringLength(32)]
    public string Theme { get; set; }
}
