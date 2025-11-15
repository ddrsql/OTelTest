using VoloAbp.OTel.Localization;
using Volo.Abp.Application.Services;

namespace VoloAbp.OTel;

/* Inherit your application services from this class.
 */
public abstract class OTelAppService : ApplicationService, IOTelActivityEnabled
{
    protected OTelAppService()
    {
        LocalizationResource = typeof(OTelResource);
    }
}
