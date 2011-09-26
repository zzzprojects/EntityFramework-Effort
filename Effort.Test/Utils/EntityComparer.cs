using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Data.Metadata.Edm;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MMDB.EntityFrameworkProvider.UnitTests.Utils
{
    public class EntityComparer : IEqualityComparer
    {
        private static Dictionary<Type, PropertyInfo[]> properties = new Dictionary<Type,PropertyInfo[]>();

        public EntityComparer()
        {

        }

        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }

            return this.CompareEqualityCore(x, y);
        }

        private bool CompareEqualityCore(object x, object y)
        {
            PropertyInfo[] props;

            if (!properties.TryGetValue(x.GetType(), out props))
            {
                props = x.GetType().GetProperties();
                properties.Add(x.GetType(), props);
            }

            using (EntityReferenceCollection refCol = new EntityReferenceCollection())
            {
                // The comparsion already has began, so skip
                if (refCol.Has(x, y))
                {
                    return true;
                }

                // Register comparsion
                refCol.Add(x, y);



                foreach (var prop in props)
                {
                    Type propType = prop.PropertyType;

                    object xVal = prop.GetValue(x, null);
                    object yVal = prop.GetValue(y, null);

                    //Null check
                    if (xVal == null || yVal == null)
                    {
                        if (xVal == null && yVal == null)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    var comparer = EqualityComparers.Create(propType);

                    if (!comparer.Equals(xVal, yVal))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
