
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Effort.Test.Data
{
    
    public class Territories
    {
        public virtual string TerritoryID { get; set; }
    
        public virtual string TerritoryDescription { get; set; }
    
        public virtual int RegionID { get; set; }
    
        public virtual Region Region { get; set; }
    
        public virtual ICollection<Employees> Employees { get; set; }
    
    
    }
}
