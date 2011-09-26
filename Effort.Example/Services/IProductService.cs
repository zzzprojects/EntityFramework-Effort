using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.TddTest.Models;

namespace MMDB.EntityFrameworkProvider.TddTest.Services
{
    public interface IProductService
    {
        IList<Products> GetAllProducts();

        Products GetProduct(int id);

        void DeleteProduct(Products product);
    }
}
