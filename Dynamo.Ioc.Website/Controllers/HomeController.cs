using System.Web.Mvc;

namespace Dynamo.Ioc.Website.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Examples()
		{
			return View();
		}
	}
}
