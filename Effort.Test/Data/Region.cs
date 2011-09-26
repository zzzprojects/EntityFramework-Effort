
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Region
    {
        public virtual int RegionID { get; set; }
    
        public virtual string RegionDescription { get; set; }
    
        public virtual ICollection<Territories> Territories { get; set; }
    
    
    }
}
