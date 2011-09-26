using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public class ContextHandler : IDisposable
    {
        private Stack<ContextHandler> owner;
        private Context context;

        public ContextHandler(Context context, Stack<ContextHandler> owner)
        {
            this.context = context;
            this.owner = owner;
        }

        public Context Context
        {
            get { return this.context; }
        }

        public void Dispose()
        {
            if (this.owner.Peek() != this)
            {
                throw new InvalidOperationException();
            }

            this.owner.Pop();
        }
    }
}
