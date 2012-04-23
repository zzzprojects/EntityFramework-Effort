
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Effort.Test.Data
{
    
    public class TimestampSupport
    {
        public virtual int Id { get; set; }
    
        public virtual string Description { get; set; }
    
        public virtual byte[] Timestamp { get; set; }
    
    
    }
}
