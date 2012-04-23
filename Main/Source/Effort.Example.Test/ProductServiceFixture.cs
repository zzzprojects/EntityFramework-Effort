#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

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
            Assert.IsNotNull(result, "Product does not exist");
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
