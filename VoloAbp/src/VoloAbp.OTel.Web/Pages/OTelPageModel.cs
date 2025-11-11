using VoloAbp.OTel.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace VoloAbp.OTel.Web.Pages;

public abstract class OTelPageModel : AbpPageModel
{
    protected OTelPageModel()
    {
        LocalizationResourceType = typeof(OTelResource);
    }
}
