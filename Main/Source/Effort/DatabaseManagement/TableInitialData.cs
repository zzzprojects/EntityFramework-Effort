#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Helpers;

namespace Effort.DatabaseManagement
{
    internal class TableInitialData
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
