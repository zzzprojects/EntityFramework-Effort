using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using MMDB.EntityFrameworkProvider.Helpers;
using System.Reflection;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class TableInitialData
    {
        private List<object> data;
        private IEntityCloner dataCloner;

        public TableInitialData(IEnumerable<object> data)
        {
            this.data = data.ToList();

            if (this.data.Count > 0)
            {
                Type entityType = this.data[0].GetType();

                this.dataCloner = Activator.CreateInstance(typeof(EntityCloner<>).MakeGenericType(entityType))
                        as IEntityCloner;
            }
        }


        public IEnumerable<object> GetClonedInitialData()
        {
            foreach (object obj in this.data)
            {
                yield return this.dataCloner.Clone(obj);
            }
        }


        private interface IEntityCloner
        {
            object Clone(object obj);
        }

        private class EntityCloner<T> : IEntityCloner
            where T : class
        {
            private Func<T, T> cloner;

            public EntityCloner()
            {
                PropertyInfo[] props = typeof(T).GetProperties(); 
                ParameterExpression param = Expression.Parameter(typeof(T));

                LambdaExpression initializer = LambdaExpressionHelper.CreateInitializerExpression(typeof(T), props);

                Expression initializerInvoker = Expression.Invoke(initializer, props.Select(p => Expression.Property(param, p)));

                this.cloner = Expression.Lambda<Func<T, T>>(initializerInvoker, param).Compile();
            }

            public object Clone(object obj)
            {
                T entity = obj as T;

                if (entity == null)
                {
                    return null;
                }

                return this.cloner.Invoke(entity);
            }
        }


    }
}
