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
