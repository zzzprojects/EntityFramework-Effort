using System;
using System.Web.Mvc;
using System.Web.Routing;
using Effort.Example.Models;
using Effort.Example.Services;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Web.UnityExtensions.Mvc;
using System.IO;
using Effort.DataLoaders;

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
            bool production = false;
            bool useDb = false;

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            IUnityContainer di = new UnityContainer();

            // Register data services
            di.RegisterType<IProductService, ProductService>();
            di.RegisterType<ICategoryService, CategoryService>();

            

            if (production || useDb)
            {
                // Register normal object context type
                di.RegisterType<INorthwindEntities, NorthwindEntities>(new InjectionConstructor());
            }
            else
            {
                // Path of the CSV files
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\..\\Effort.Test\\Data\\Initial\\Northwind");
                // Define emulator object context type
                Type emulator = ObjectContextFactory.CreatePersistentType<NorthwindEntities>(new CsvDataLoader(path));

                // Register emulator object context type
                di.RegisterType(typeof(INorthwindEntities), emulator);
            }

            // Initialize Mvc application bootstrapper
            UnityMvcBootstrapper bootstrapper = new UnityMvcBootstrapper(di);
        }
    }
}