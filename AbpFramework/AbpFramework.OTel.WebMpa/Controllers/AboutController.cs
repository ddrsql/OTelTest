using System.Web.Mvc;

namespace AbpFramework.OTel.WebMpa.Controllers
{
    public class AboutController : OTelControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}