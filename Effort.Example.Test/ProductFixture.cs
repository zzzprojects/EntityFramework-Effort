using Effort.Example.Models;
using Effort.Example.Services;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Example.Test
{
    [TestClass]
    public class ProductFixture
    {
        private IUnityContainer dependencies;

        [TestInitialize]
        public void Initialize()
        {
            UnityContainer di = new UnityContainer();

            // Register fake object context
            di.RegisterType(typeof(NorthwindEntities), TypeStore.EmulatorContext);

            this.dependencies = di;
        }


        [TestMethod]
        public void ProductExist()
        {
            // Arrange
            ProductService service = dependencies.Resolve<ProductService>();

            // Act
            var result = service.GetProduct(1);

            // Assert
            Assert.AreNotEqual(result, null, "Product does not exist");
        }


        [TestMethod]
        public void AllProduct()
        {
            // Arrange
            ProductService service = dependencies.Resolve<ProductService>();

            // Act
            var result = service.GetAllProducts();
            
            // Assert
            Assert.AreEqual(result.Count, 77, "Size of the result set");

        }

        [TestMethod]
        public void DeleteProduct()
        {
            // Arrange
            ProductService service = dependencies.Resolve<ProductService>();

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
            ProductService service = dependencies.Resolve<ProductService>();

            // Act
            Product product = service.GetProduct(1);

            // Assert
            Assert.AreEqual(product.ProductName, "Chai", "Name of the product");
        }



    }
}
