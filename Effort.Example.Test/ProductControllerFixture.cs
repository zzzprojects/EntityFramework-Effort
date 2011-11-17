using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Example.Models;
using Microsoft.Practices.Unity;
using Effort.Example.Services;
using Effort.Example.Controllers;
using System.Web.Mvc;

namespace Effort.Example.Test
{
    [TestClass]
    public class ProductControllerFixture
    {
        private IUnityContainer dependencies;

        [TestInitialize]
        public void Initialize()
        {
            UnityContainer di = new UnityContainer();

            // Register fake object context
            di.RegisterType(typeof(NorthwindEntities), TypeStore.EmulatorContext);
            di.RegisterType(typeof(IProductService), typeof(ProductService));

            this.dependencies = di;
        }

        [TestMethod]
        public void ProductNotFound()
        {
            // Arrange
            ProductController controller = this.dependencies.Resolve<ProductController>();

            // Act
            var result = controller.Details(78) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result is not ViewResult");
            Assert.AreEqual(result.ViewName, "NotFound", "The view is not 'NotFound'"); 
        }
    }
}
