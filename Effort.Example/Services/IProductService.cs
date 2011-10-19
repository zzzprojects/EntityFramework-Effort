using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Example.Models;

namespace Effort.Example.Services
{
    public interface IProductService
    {
        IList<Product> GetAllProducts();

        Product GetProduct(int id);

        void DeleteProduct(Product product);
    }
}
