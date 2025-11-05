using Abp.AspNetCore.Mvc.Authorization;
using AbpCore.OTel.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AbpCore.OTel.Web.Controllers;

[AbpMvcAuthorize]
public class HomeController : OTelControllerBase
{
    public ActionResult Index()
    {
        return View();
    }
}
