
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Effort.Test.Data
{
    
    public class CustomerDemographics
    {
        public virtual string CustomerTypeID { get; set; }
    
        public virtual string CustomerDesc { get; set; }
    
        public virtual ICollection<Customers> Customers { get; set; }
    
    
    }
}
