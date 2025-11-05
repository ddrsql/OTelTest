using AbpCore.OTel.Configuration.Dto;
using System.Threading.Tasks;

namespace AbpCore.OTel.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
