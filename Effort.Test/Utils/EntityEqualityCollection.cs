using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.UnitTests.Utils
{
    public class EntityReferenceCollection : IDisposable
    {
        [ThreadStatic]
        private static int counter;
        [ThreadStatic]
        private static List<EntityComparsion> comparsions;

        [ThreadStatic]
        private static Stack<List<EntityComparsion>> comparsionsStack;

        public EntityReferenceCollection()
        {
            if (comparsionsStack == null)
            {
                comparsionsStack = new Stack<List<EntityComparsion>>();
            }

            if (counter == 0)
            {
                comparsions = new List<EntityComparsion>();
                comparsions = new List<EntityComparsion>();
            }
            else
            {
                comparsionsStack.Push(comparsions);

                comparsions = comparsions.ToList();
            }

            counter++;
        }


        public void Add(object x, object y)
        {
            comparsions.Add(new EntityComparsion(x, y));
        }

        public bool Has(object x, object y)
        {
            for (int i = 0; i < comparsions.Count; i++)
            {
                if (comparsions[i].Equals(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            if (counter > 1)
            {
                comparsions = comparsionsStack.Pop();
            }

            counter--;
        }
    }

    public class EntityComparsion
    {
        private object x;
        private object y;

        public EntityComparsion(object x, object y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(object x, object y)
        {
            return object.ReferenceEquals(this.x, x) && object.ReferenceEquals(this.y, y);
        }
    }
}
