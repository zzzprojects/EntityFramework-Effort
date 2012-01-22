using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Effort.Example.Models;
using Microsoft.Practices.Unity;

namespace Effort.Example.Services
{
    public class CategoryService : ICategoryService
    {
        [Dependency]
        public INorthwindEntities Context { set; get; }

        public IList<Category> GetAllCategories()
        {
            return this.Context.Categories.ToList();
        }


        public void InsertCategory(Category category)
        {
            this.Context.Categories.AddObject(category);

            this.Context.SaveChanges();
        }
    }
}