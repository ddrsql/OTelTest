using Abp.Application.Services;
using AbpCore.OTel.Sessions.Dto;
using System.Threading.Tasks;

namespace AbpCore.OTel.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
