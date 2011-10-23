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
    public class CategoryFixture
    {
        private IUnityContainer dependencies;

        [TestInitialize]
        public void Initialize()
        {
            UnityContainer di = new UnityContainer();

            // Register fake object context
            di.RegisterType(typeof(NorthwindEntities), EmulatorFactory.Create());

            this.dependencies = di;
        }

        [TestMethod]
        public void InsertCategory()
        {
            // Arrange
            CategoryService service = this.dependencies.Resolve<CategoryService>();

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
            CategoryService service = this.dependencies.Resolve<CategoryService>();

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
