using VoloAbp.OTel.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace VoloAbp.OTel.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class OTelController : AbpControllerBase
{
    protected OTelController()
    {
        LocalizationResource = typeof(OTelResource);
    }
}
