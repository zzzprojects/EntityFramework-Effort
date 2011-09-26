using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MMDB.EntityFrameworkProvider.UnitTests.Utils
{
    class TypeSafeEnumerator<T> : IEnumerable<T>, IEnumerator<T>
    {
        private IEnumerator e;

        public TypeSafeEnumerator(IEnumerable e)
        {
            this.e = e.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get { return (T)this.e.Current; }
        }

        public void Dispose()
        {
            
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.e.Current; }
        }

        public bool MoveNext()
        {
            return this.e.MoveNext();
        }

        public void Reset()
        {
            this.e.Reset();
        }
    }
}
