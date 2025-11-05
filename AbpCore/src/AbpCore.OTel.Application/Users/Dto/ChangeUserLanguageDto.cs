using System.ComponentModel.DataAnnotations;

namespace AbpCore.OTel.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}