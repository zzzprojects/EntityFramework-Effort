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
using Rhino.Mocks;

namespace Effort.Example.Test
{
    [TestClass]
    public class ProductControllerFixture
    {
        [TestMethod]
        public void ProductNotFound()
        {
            // Arrange
            IProductService productServiceMock = MockRepository.GenerateMock<IProductService>();

            productServiceMock
                .Expect(x => x.GetProduct(1))
                .Return(null);

            IUnityContainer di = new UnityContainer()
                .RegisterInstance(typeof(IProductService), productServiceMock);

            ProductController controller = di.Resolve<ProductController>();

            // Act
            var result = controller.Details(1) as ViewResult;

            // Assert
            productServiceMock.VerifyAllExpectations();
            Assert.IsNotNull(result, "Result is not ViewResult");
            Assert.AreEqual(result.ViewName, "NotFound", "The view is not 'NotFound'"); 
        }
        
        [TestMethod]
        public void ProductNotFoundIntegration()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(IProductService), typeof(ProductService))
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);
                
            ProductController controller = di.Resolve<ProductController>();

            // Act
            var result = controller.Details(78) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result is not ViewResult");
            Assert.AreEqual(result.ViewName, "NotFound", "The view is not 'NotFound'"); 
        }
    }
}
