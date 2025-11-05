using Abp.AutoMapper;
using AbpCore.OTel.Sessions.Dto;

namespace AbpCore.OTel.Web.Views.Shared.Components.TenantChange;

[AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
public class TenantChangeViewModel
{
    public TenantLoginInfoDto Tenant { get; set; }
}
