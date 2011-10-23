using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Effort.Example.Models;

namespace Effort.Example.Services
{
    public interface ICategoryService
    {
        IList<Category> GetAllCategories();

        void InsertCategory(Category category);
    }
}