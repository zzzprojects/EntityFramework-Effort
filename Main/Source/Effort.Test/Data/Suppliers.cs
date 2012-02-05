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
    
    public class Suppliers
    {
        public virtual int SupplierID { get; set; }
    
        public virtual string CompanyName { get; set; }
    
        public virtual string ContactName { get; set; }
    
        public virtual string ContactTitle { get; set; }
    
        public virtual string Address { get; set; }
    
        public virtual string City { get; set; }
    
        public virtual string Region { get; set; }
    
        public virtual string PostalCode { get; set; }
    
        public virtual string Country { get; set; }
    
        public virtual string Phone { get; set; }
    
        public virtual string Fax { get; set; }
    
        public virtual string HomePage { get; set; }
    
        public virtual ICollection<Products> Products { get; set; }
    
    
    }
}
