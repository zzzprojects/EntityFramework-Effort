
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
