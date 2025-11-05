using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AbpCore.OTel.Controllers
{
    public abstract class OTelControllerBase : AbpController
    {
        protected OTelControllerBase()
        {
            LocalizationSourceName = OTelConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
