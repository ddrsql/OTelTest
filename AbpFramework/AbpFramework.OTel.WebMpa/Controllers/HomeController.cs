using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;

namespace AbpFramework.OTel.WebMpa.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : OTelControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}