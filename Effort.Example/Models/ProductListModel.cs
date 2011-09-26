using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMDB.EntityFrameworkProvider.TddTest.Models
{
    public class ProductListModel
    {
        public IList<Products> Products { get; set; }
    }
}