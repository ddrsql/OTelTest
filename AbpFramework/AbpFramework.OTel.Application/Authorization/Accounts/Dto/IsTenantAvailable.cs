using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;

namespace AbpFramework.OTel.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [MaxLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}
