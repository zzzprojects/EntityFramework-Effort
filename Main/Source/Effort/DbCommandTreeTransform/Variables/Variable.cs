using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Effort.DbCommandTreeTransform.Variables
{
    internal class Variable
    {
        public Expression Expression { set; get; }

        public string Name { set; get; }
    }
}
