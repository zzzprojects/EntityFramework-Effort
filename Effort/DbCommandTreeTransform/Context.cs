using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Effort.DbCommandTreeTransform
{
    internal class Context
    {
        public Expression SourceExpression { set; get; }
        public string Name { set; get; }
    }
}
