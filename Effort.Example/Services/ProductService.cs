using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Effort.Example.Models;
using Microsoft.Practices.Unity;

namespace Effort.Example.Services
{
    public class ProductService : IProductService
    {
        [Dependency]
        public NorthwindEntities Context { set; get; }

        public IList<Products> GetAllProducts()
        {
            return Context.Products.ToList();
        }

        public Products GetProduct(int id)
        {
            return Context.Products.FirstOrDefault(p => p.ProductID == id);
        }

        public void DeleteProduct(Products product)
        {
            Context.Products.Attach(product);

            Context.Products.DeleteObject(product);
            Context.SaveChanges();
        }
    }
}