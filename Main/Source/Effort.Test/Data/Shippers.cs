
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Effort.Test.Data
{
    
    public class Shippers
    {
        public virtual int ShipperID { get; set; }
    
        public virtual string CompanyName { get; set; }
    
        public virtual string Phone { get; set; }
    
        public virtual ICollection<Orders> Orders { get; set; }
    
    
    }
}
