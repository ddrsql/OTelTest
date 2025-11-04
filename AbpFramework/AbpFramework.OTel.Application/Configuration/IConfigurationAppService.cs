using System.Threading.Tasks;
using Abp.Application.Services;
using AbpFramework.OTel.Configuration.Dto;

namespace AbpFramework.OTel.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}