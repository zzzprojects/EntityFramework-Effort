using System;
using System.Web.Mvc;
using System.Web.Routing;
using Effort.Example.Models;
using Effort.Example.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Web.UnityExtensions.Mvc;
using System.IO;

namespace Effort.Example
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            IUnityContainer di = new UnityContainer();

            di.RegisterType<IProductService, ProductService>();


            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\Effort.Example.Test\\Data");
            Type emulator = ObjectContextFactory.CreateEmulator<NorthwindEntities>(path, true);

            di.RegisterType(typeof(NorthwindEntities), emulator);
            //di.RegisterType<NorthwindEntities, NorthwindEntities>(new InjectionConstructor());

            UnityMvcBootstrapper bootstrapper = new UnityMvcBootstrapper(di);
        }
    }
}