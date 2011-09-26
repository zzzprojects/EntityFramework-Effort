
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Products
    {
        public virtual int ProductID { get; set; }
    
        public virtual string ProductName { get; set; }
    
        public virtual Nullable<int> SupplierID { get; set; }
    
        public virtual Nullable<int> CategoryID { get; set; }
    
        public virtual string QuantityPerUnit { get; set; }
    
        public virtual Nullable<decimal> UnitPrice { get; set; }
    
        public virtual Nullable<short> UnitsInStock { get; set; }
    
        public virtual Nullable<short> UnitsOnOrder { get; set; }
    
        public virtual Nullable<short> ReorderLevel { get; set; }
    
        public virtual bool Discontinued { get; set; }
    
        public virtual Categories Categories { get; set; }
    
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    
        public virtual Suppliers Suppliers { get; set; }
    
    
    }
}
