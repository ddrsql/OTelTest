using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpFramework.OTel.MultiTenancy.Dto;

namespace AbpFramework.OTel.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
