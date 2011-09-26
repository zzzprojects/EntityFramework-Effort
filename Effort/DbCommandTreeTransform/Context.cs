using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public class Context
    {
        public Expression SourceExpression { set; get; }
        public string Name { set; get; }
    }
}
