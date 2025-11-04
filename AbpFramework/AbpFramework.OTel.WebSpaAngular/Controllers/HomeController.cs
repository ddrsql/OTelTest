using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace AbpFramework.OTel.WebSpaAngular.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : OTelControllerBase
    {
        public ActionResult Index()
        {
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }
	}
}