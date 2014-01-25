// --------------------------------------------------------------------------------------------
// <copyright file="EdmHelper.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.Common
{
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;

    /// <summary>
    ///     Providers helper method for EDM types.
    /// </summary>
    internal static class EdmHelper
    {
        /// <summary>
        ///     Returns the name of the table that is represented by the specified entity set.
        /// </summary>
        /// <param name="entitySet"> The entity set. </param>
        /// <returns >The name of the table represented by the entity set. </returns>
        public static string GetTableName(this EntitySetBase entitySet)
        {
            MetadataProperty property = entitySet
               .MetadataProperties
               .FirstOrDefault(p => p.Name == "Table");

            if (property == null)
            {
                return entitySet.Name;
            }

            return property.Value as string ?? entitySet.Name;
        }

        /// <summary>
        ///     Returns the name of the table column that is represented by the specified
        ///     member.
        /// </summary>
        /// <param name="member"> The member. </param>
        /// <returns> The name of the table column represented by the member. </returns>
        public static string GetColumnName(this EdmMember member)
        {
            return member.Name;
        }
    }
}
