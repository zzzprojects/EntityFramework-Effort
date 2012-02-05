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
    
    public class Employees
    {
        public virtual int EmployeeID { get; set; }
    
        public virtual string LastName { get; set; }
    
        public virtual string FirstName { get; set; }
    
        public virtual string Title { get; set; }
    
        public virtual string TitleOfCourtesy { get; set; }
    
        public virtual Nullable<System.DateTime> BirthDate { get; set; }
    
        public virtual Nullable<System.DateTime> HireDate { get; set; }
    
        public virtual string Address { get; set; }
    
        public virtual string City { get; set; }
    
        public virtual string Region { get; set; }
    
        public virtual string PostalCode { get; set; }
    
        public virtual string Country { get; set; }
    
        public virtual string HomePhone { get; set; }
    
        public virtual string Extension { get; set; }
    
        public virtual byte[] Photo { get; set; }
    
        public virtual string Notes { get; set; }
    
        public virtual Nullable<int> ReportsTo { get; set; }
    
        public virtual string PhotoPath { get; set; }
    
        public virtual ICollection<Employees> Employees1 { get; set; }
    
        public virtual Employees Employees2 { get; set; }
    
        public virtual ICollection<Orders> Orders { get; set; }
    
        public virtual ICollection<Territories> Territories { get; set; }
    
    
    }
}
