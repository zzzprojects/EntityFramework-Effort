using Effort.Example.Models;
using Effort.Example.Services;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Example.Test.UnitTest
{
    [TestClass]
    public class ProductFixture
    {
        private IUnityContainer dependencies;

        [TestInitialize]
        public void Initialize()
        {
            UnityContainer di = new UnityContainer();

            ////di.RegisterType(typeof(NorthwindEntities), new InjectionConstructor());
            di.RegisterType(typeof(NorthwindEntities), EmulatorFactory.Create());

            this.dependencies = di;
        }

        [TestMethod]
        public void AllProduct()
        {
            // Arrange
            ProductService service = dependencies.Resolve<ProductService>();

            // Act
            var result = service.GetAllProducts();
            
            // Assert
            Assert.AreEqual(result.Count, 81, "Size of the result set");

        }

        [TestMethod]
        public void DeleteProduct()
        {
            // Arrange
            ProductService service = dependencies.Resolve<ProductService>();

            Products product = new Products();
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
            Products product = service.GetProduct(1);

            // Assert
            Assert.AreEqual(product.ProductName, "Chai", "Name of the product");
        }



    }
}
