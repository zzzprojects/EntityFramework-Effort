
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MMDB.EntityFrameworkProvider.UnitTests.Data
{
    
    public class Categories
    {
        public virtual int CategoryID { get; set; }
    
        public virtual string CategoryName { get; set; }
    
        public virtual string Description { get; set; }
    
        public virtual byte[] Picture { get; set; }
    
        public virtual ICollection<Products> Products { get; set; }
    
    
    }
}
