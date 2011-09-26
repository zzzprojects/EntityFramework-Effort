using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Web.UnityExtensions.Mvc;
using Microsoft.Practices.Unity;
using MMDB.EntityFrameworkProvider.TddTest.Services;
using MMDB.EntityFrameworkProvider.TddTest.Models;
using MMDB.EntityFrameworkProvider.Tdd;

namespace MMDB.EntityFrameworkProvider.TddTest
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

            Type emulator = ObjectContextFactory.CreateEmulator<NorthwindEntities>(@"C:\Users\user\Desktop\db", true);
            di.RegisterType(typeof(NorthwindEntities), emulator);
            //di.RegisterType<NorthwindEntities, NorthwindEntities>(new InjectionConstructor());

            UnityMvcBootstrapper bootstrapper = new UnityMvcBootstrapper(di);
        }
    }
}