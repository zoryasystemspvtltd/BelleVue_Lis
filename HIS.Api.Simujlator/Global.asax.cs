using HIS.Api.Simujlator.App_Start;
using LIS.Logger;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HIS.Api.Simujlator
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var container = SimpleInjectorInitializer.Initialize();
            var logger = container.GetInstance<ILogger>();
            GlobalScheduler.StartScheduler(logger);
        }
    }
}
