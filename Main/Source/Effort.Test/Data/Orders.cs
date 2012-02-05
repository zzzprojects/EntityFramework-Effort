#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Effort.Test.Data
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
