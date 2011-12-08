using Effort.Example.Models;
using Effort.Example.Services;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Example.Test
{
    [TestClass]
    public class ProductServiceFixture
    {

        [TestMethod]
        public void ProductExist()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            ProductService service = di.Resolve<ProductService>();

            // Act
            var result = service.GetProduct(1);

            // Assert
            Assert.AreNotEqual(result, null, "Product does not exist");
        }


        [TestMethod]
        public void AllProduct()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            ProductService service = di.Resolve<ProductService>();

            // Act
            var result = service.GetAllProducts();
            
            // Assert
            Assert.AreEqual(result.Count, 77, "Size of the result set");

        }

        [TestMethod]
        public void DeleteProduct()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            ProductService service = di.Resolve<ProductService>();

            Product product = new Product();
            product.ProductID = 1;

            // Act
            service.DeleteProduct(product);

            // Assert
            product = service.GetProduct(1);

            Assert.IsNull(product, "Product should not exist");
        }

        [TestMethod]
        public void GetProduct()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            ProductService service = di.Resolve<ProductService>();

            // Act
            Product product = service.GetProduct(1);

            // Assert
            Assert.AreEqual(product.ProductName, "Chai", "Name of the product");
        }



    }
}
