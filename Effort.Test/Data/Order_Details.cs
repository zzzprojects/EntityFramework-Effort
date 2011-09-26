
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Order_Details
    {
        public virtual int OrderID { get; set; }
    
        public virtual int ProductID { get; set; }
    
        public virtual decimal UnitPrice { get; set; }
    
        public virtual short Quantity { get; set; }
    
        public virtual float Discount { get; set; }
    
        public virtual Orders Orders { get; set; }
    
        public virtual Products Products { get; set; }
    
    
    }
}
