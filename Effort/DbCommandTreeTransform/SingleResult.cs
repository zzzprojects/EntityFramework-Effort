using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public class SingleResult<T> : IQueryable<T>
    {
        private List<T> innerList;
        private IQueryable<T> innerQueryable; 

        public SingleResult(T item)
        {
            this.innerList = new List<T>();
            this.innerList.Add(item);
            this.innerQueryable = this.innerList.AsQueryable();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.innerQueryable.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.innerQueryable.GetEnumerator();
        }

        public Type ElementType
        {
            get { return this.innerQueryable.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return this.innerQueryable.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.innerQueryable.Provider; }
        }
    }
}
