using Abp.AutoMapper;
using AbpFramework.OTel.Sessions.Dto;

namespace AbpFramework.OTel.WebSpaAngular.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}