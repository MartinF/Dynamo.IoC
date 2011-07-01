using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dynamo.Ioc.Website
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			//AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			//routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("Home", "", new { controller = "Home", action = "Index" });
			routes.MapRoute("Examples", "examples", new { controller = "Home", action = "Examples" });
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}