
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Customers
    {
        public virtual string CustomerID { get; set; }
    
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
    
        public virtual ICollection<Orders> Orders { get; set; }
    
        public virtual ICollection<CustomerDemographics> CustomerDemographics { get; set; }
    
    
    }
}
