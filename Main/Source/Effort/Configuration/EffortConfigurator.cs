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
using System.Data.Objects;
using Effort.DatabaseManagement;
using MMDB;
using System.Linq.Expressions;
using System.Reflection;

namespace Effort.Configuration
{
    public static class EffortConfigurator
    {
        ////public static void UpdateStatistics<TObjectSet>(ObjectContext context,
        ////    Expression<Func<TObjectSet>> tableSelector)
        ////{
        ////    if (context.Connection.State == System.Data.ConnectionState.Closed)
        ////        context.Connection.Open();
        ////    var table = getTable<TObjectSet>(context, tableSelector);

        ////    table.Statistics.UpdateAll();
        ////}

        ////private static MMDB.Table.IReflectionTable getTable<TObjectSet>(ObjectContext context, Expression<Func<TObjectSet>> tableSelector)
        ////{
        ////    var connectionString = context.Connection.ConnectionString;
        ////    var db = DbInstanceStore.GetDbInstance(connectionString, null);
        ////    var tableName = ((MemberExpression)tableSelector.Body).Member.Name;
        ////    var table = db.GetTable(tableName);
        ////    return table;
        ////}
        ////public static void UpdateStatistics<TEntity, TColumn>(ObjectContext context,
        ////    Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
        ////        where TEntity : class
        ////{
        ////    var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
        ////    var column = getColumn<TEntity, TColumn>(columnSelector);

        ////    table.Statistics.Update(column);
        ////}

        ////private static MemberInfo getColumn<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> columnSelector)
        ////{
        ////    MemberExpression expr = columnSelector.Body as MemberExpression;
        ////    if (columnSelector is UnaryExpression)
        ////    {
        ////        expr = (columnSelector as UnaryExpression).Operand as MemberExpression;
        ////    }
        ////    var columnName = expr.Member.Name;
        ////    var columnMember = typeof(TEntity).GetProperty(columnName);

        ////    return columnMember;
        ////}
        ////public static void CreateStatistics<TEntity, TColumn>(ObjectContext context,
        ////    Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
        ////    where TEntity : class
        ////{
        ////    var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
        ////    var column = getColumn<TEntity, TColumn>(columnSelector);

        ////    table.Statistics.CreateIntegerStatistics(column.Name);
        ////}

        ////public static void Test<TEntity, TColumn>(ObjectContext context,
        ////    Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
        ////        where TEntity : class
        ////{
        ////    var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
        ////    var column = getColumn<TEntity, TColumn>(columnSelector);
        ////}
    }
}
