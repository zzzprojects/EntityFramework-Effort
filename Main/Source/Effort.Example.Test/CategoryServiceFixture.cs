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
using Microsoft.Practices.Unity;
using Effort.Example.Models;
using Effort.Example.Services;

namespace Effort.Example.Test
{
    [TestClass]
    public class CategoryServiceFixture
    {
        [TestMethod]
        public void InsertCategory()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            CategoryService service = di.Resolve<CategoryService>();

            Category category = new Category
            {
                CategoryName = "New category"
            };

            // Act
            service.InsertCategory(category);

            // Assert
            Assert.AreNotEqual(category.CategoryID, 0, "Generated ID of the category");
        }

        [TestMethod]
        public void InsertTwoCategories()
        {
            // Arrange
            IUnityContainer di = new UnityContainer()
                .RegisterType(typeof(INorthwindEntities), TypeStore.EmulatorContext);

            CategoryService service = di.Resolve<CategoryService>();

            Category category1 = new Category
            {
                CategoryName = "New category 1"
            };

            Category category2 = new Category
            {
                CategoryName = "New category 2"
            };

            // Act
            service.InsertCategory(category1);
            service.InsertCategory(category2);

            // Assert
            Assert.IsTrue(category1.CategoryID < category2.CategoryID, "Incrementing category ID");
        }

    }
}
