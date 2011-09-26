using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MMDB.EntityFrameworkProvider.TddTest.Services;
using Microsoft.Practices.Unity;
using MMDB.EntityFrameworkProvider.TddTest.Models;

namespace MMDB.EntityFrameworkProvider.TddTest.Controllers
{

    public class ProductController : Controller
    {
        [Dependency]
        public IProductService ProductService { get; set; }

        public ActionResult List()
        {
            var result = this.ProductService.GetAllProducts();
            var model = new ProductListModel();

            model.Products = result;

            return View(model);
        }


        public ActionResult Details(int id)
        {
            var result = this.ProductService.GetProduct(id);

            return View(result);
        }

    }
}
