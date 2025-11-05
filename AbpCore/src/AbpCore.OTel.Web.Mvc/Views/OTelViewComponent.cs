using Abp.AspNetCore.Mvc.ViewComponents;

namespace AbpCore.OTel.Web.Views;

public abstract class OTelViewComponent : AbpViewComponent
{
    protected OTelViewComponent()
    {
        LocalizationSourceName = OTelConsts.LocalizationSourceName;
    }
}
