using HIS.Api.Simujlator.DataAccess;
using LIS.Logger;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

namespace HIS.Api.Simujlator
{
    public class SimpleInjectorInitializer
    {
        public static Container Initialize()
        {
            var container = GetInitializeContainer();

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }

        public static Container GetInitializeContainer()                 
        {            
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Register<ILogger, Logger>(Lifestyle.Singleton);
            container.Register<ITestRequisitionRepository, TestRequisitionRepository>(Lifestyle.Scoped);            
            
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            return container;
        }
        
    }
}