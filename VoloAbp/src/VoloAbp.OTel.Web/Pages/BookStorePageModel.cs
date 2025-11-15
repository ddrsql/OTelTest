using VoloAbp.OTel.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace VoloAbp.OTel.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class BookStorePageModel : AbpPageModel
{
    protected BookStorePageModel()
    {
        LocalizationResourceType = typeof(OTelResource);
    }
}
