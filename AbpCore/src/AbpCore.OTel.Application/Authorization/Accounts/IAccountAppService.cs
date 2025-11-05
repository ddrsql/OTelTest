using Abp.Application.Services;
using AbpCore.OTel.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace AbpCore.OTel.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
