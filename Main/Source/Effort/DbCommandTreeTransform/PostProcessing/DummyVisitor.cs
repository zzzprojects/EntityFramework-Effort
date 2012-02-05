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
using System.Text;
using System.Linq.Expressions;

namespace Effort.DbCommandTreeTransform.PostProcessing
{
    /// <summary>
    /// This is visitor is for testing purpose
    /// </summary>
    internal class DummyVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);

            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
            {
                visitor a = new visitor();

                Expression predicate = node.Arguments[1];
                a.Visit(predicate);

                var debug = typeof(DummyVisitor).GetMethod("Debug");

                var where = typeof(Queryable).GetMethods().Where(m => m.Name == "Where").FirstOrDefault().MakeGenericMethod(node.Method.GetGenericArguments()[0]);

                var lamda = (predicate as UnaryExpression).Operand as LambdaExpression;

                var debugcall = Expression.Call(
                                debug,
                                lamda.Body,
                                a.Parameters[0],
                                a.Parameters[1]);


                var res = Expression.Call(
                    where,
                    node.Arguments[0],
                    Expression.Quote(
                        Expression.Lambda(debugcall, lamda.Parameters[0]))
                        );

                return res;
            }

            return base.VisitMethodCall(node);
        }

        public static bool Debug(bool result, object param1, object param2)
        {
            var id = param1.GetType().GetProperty("EmployeeID");
            var reportTo = param1.GetType().GetProperty("ReportsTo");

            if (id != null && reportTo != null)
            {
                //Console.WriteLine("{0}, {1}", id.GetValue(param1, null), reportTo.GetValue(param2, null));
            }

            return result;
        }



        private class visitor : ExpressionVisitor
        {
            public List<ParameterExpression> Parameters { get; set; }

            public visitor()
            {
                this.Parameters = new List<ParameterExpression>();
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (!this.Parameters.Contains(node))
                {
                    this.Parameters.Add(node);
                }

                return base.VisitParameter(node);
            }
        }
    }


}
