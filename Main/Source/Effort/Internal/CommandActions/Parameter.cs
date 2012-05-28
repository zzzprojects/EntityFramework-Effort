using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Internal.DbCommandActions
{
    internal class Parameter
    {
        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }
}
