using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Effort.Example.Models
{
    public class A : NorthwindEntities
    {
        public A() : base(EntityConnectionFactory.CreateEmulator(""))
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                this.Connection.Dispose();
            }
        }
    }

    public class ProductListModel
    {
        public IList<Product> Products { get; set; }
    }
}