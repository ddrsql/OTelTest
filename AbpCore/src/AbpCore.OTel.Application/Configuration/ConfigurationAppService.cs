using Abp.Authorization;
using Abp.Runtime.Session;
using AbpCore.OTel.Configuration.Dto;
using System.Threading.Tasks;

namespace AbpCore.OTel.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : OTelAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
