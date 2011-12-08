using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Effort.Example.Models;
using Microsoft.Practices.Unity;
using System.IO;

namespace Effort.Example.Services
{
    public class ProductService : IProductService
    {
        [Dependency]
        public INorthwindEntities Context { set; get; }

        public IList<Product> GetAllProducts()
        {
            return Context.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return Context.Products.FirstOrDefault(p => p.ProductID == id);
        }

        public void DeleteProduct(Product product)
        {
            Context.Products.Attach(product);

            Context.Products.DeleteObject(product);
            Context.SaveChanges();
        }
    }

}