using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AbpFramework.OTel.Configuration.Dto;

namespace AbpFramework.OTel.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : OTelAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
