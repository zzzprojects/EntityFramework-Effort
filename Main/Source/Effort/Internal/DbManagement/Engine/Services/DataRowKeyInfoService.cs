// --------------------------------------------------------------------------------------------
// <copyright file="DataRowKeyInfoService.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbManagement.Engine.Services
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.TypeGeneration;
    using NMemory.Indexes;
    using NMemory.Services.Contracts;

    internal class DataRowKeyInfoService : IKeyInfoService
    {
        public bool TryCreateKeyInfo<TEntity, TKey>(
            Expression<Func<TEntity, TKey>> keySelector, 
            out IKeyInfo<TEntity, TKey> result) 
            where TEntity : class
        {
            Expression body = keySelector.Body;
            Type keyType = body.Type;

            if (!typeof(DataRow).IsAssignableFrom(body.Type))
            {
                result = null;
                return false;
            }

            IKeyInfoHelper helper = DataRowKeyInfo<TEntity, TKey>.KeyInfoHelper;

            MemberInfo[] keyMembers;
            if (!helper.TryParseKeySelectorExpression(keySelector.Body, true, out keyMembers))
            {
                result = null;
                return false;
            }

            result = new DataRowKeyInfo<TEntity, TKey>(keyMembers);
            return true;
        }


        public bool TryCreateKeyInfoHelper(Type keyType, out IKeyInfoHelper result)
        {
            if (!typeof(DataRow).IsAssignableFrom(keyType))
            {
                result = null;
                return false;
            }

            result = new DataRowKeyInfoHelper(keyType);
            return true;
        }
    }
}
