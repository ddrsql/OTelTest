using Abp.Application.Services;
using AbpCore.OTel.MultiTenancy.Dto;

namespace AbpCore.OTel.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

