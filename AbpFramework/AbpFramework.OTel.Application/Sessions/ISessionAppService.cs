using System.Threading.Tasks;
using Abp.Application.Services;
using AbpFramework.OTel.Sessions.Dto;

namespace AbpFramework.OTel.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
