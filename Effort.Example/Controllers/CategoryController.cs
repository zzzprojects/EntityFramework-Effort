using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Effort.Example.Services;
using Effort.Example.Models;

namespace Effort.Example.Controllers
{
    public class CategoryController : Controller
    {
        [Dependency]
        public ICategoryService CategoryService { get; set; }

        public ActionResult List()
        {
            var result = this.CategoryService.GetAllCategories();
            var model = new CategoryListModel();

            model.Categories = result;

            return View(model);
        }

    }
}
