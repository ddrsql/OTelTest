using Abp.Web.Mvc.Views;

namespace AbpFramework.OTel.WebSpaAngular.Views
{
    public abstract class OTelWebViewPageBase : OTelWebViewPageBase<dynamic>
    {

    }

    public abstract class OTelWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected OTelWebViewPageBase()
        {
            LocalizationSourceName = OTelConsts.LocalizationSourceName;
        }
    }
}