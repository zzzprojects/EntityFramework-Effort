
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Orders
    {
        public virtual int OrderID { get; set; }
    
        public virtual string CustomerID { get; set; }
    
        public virtual Nullable<int> EmployeeID { get; set; }
    
        public virtual Nullable<System.DateTime> OrderDate { get; set; }
    
        public virtual Nullable<System.DateTime> RequiredDate { get; set; }
    
        public virtual Nullable<System.DateTime> ShippedDate { get; set; }
    
        public virtual Nullable<int> ShipVia { get; set; }
    
        public virtual Nullable<decimal> Freight { get; set; }
    
        public virtual string ShipName { get; set; }
    
        public virtual string ShipAddress { get; set; }
    
        public virtual string ShipCity { get; set; }
    
        public virtual string ShipRegion { get; set; }
    
        public virtual string ShipPostalCode { get; set; }
    
        public virtual string ShipCountry { get; set; }
    
        public virtual Customers Customers { get; set; }
    
        public virtual Employees Employees { get; set; }
    
        public virtual ICollection<Order_Details> Order_Details { get; set; }
    
        public virtual Shippers Shippers { get; set; }
    
    
    }
}
