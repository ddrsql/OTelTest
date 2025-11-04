using System.Threading.Tasks;
using Abp.Application.Services;
using AbpFramework.OTel.Authorization.Accounts.Dto;

namespace AbpFramework.OTel.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
