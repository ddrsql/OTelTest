using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace AbpCore.OTel.Web.Views;

public abstract class OTelRazorPage<TModel> : AbpRazorPage<TModel>
{
    [RazorInject]
    public IAbpSession AbpSession { get; set; }

    protected OTelRazorPage()
    {
        LocalizationSourceName = OTelConsts.LocalizationSourceName;
    }
}
