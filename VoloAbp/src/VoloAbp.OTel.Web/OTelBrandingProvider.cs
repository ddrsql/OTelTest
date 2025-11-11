using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using VoloAbp.OTel.Localization;

namespace VoloAbp.OTel.Web;

[Dependency(ReplaceServices = true)]
public class OTelBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<OTelResource> _localizer;

    public OTelBrandingProvider(IStringLocalizer<OTelResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
