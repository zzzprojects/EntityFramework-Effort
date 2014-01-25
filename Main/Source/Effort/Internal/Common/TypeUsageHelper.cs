// --------------------------------------------------------------------------------------------
// <copyright file="TypeUsageHelper.cs" company="Effort Team">
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

    internal static class TypeUsageHelper
    {
        public static bool TryGetMaxLength(TypeUsage typeUsage, out int maxLength)
        {
            return TryGetFacetValue(typeUsage, "MaxLength", true, out maxLength);
        }

        public static bool TryGetIsFixedLength(TypeUsage typeUsage, out bool isFixed)
        {
            return TryGetFacetValue(typeUsage, "FixedLength", false, out isFixed);
        }

        public static bool TryGetPrecision(TypeUsage typeUsage, out byte precision)
        {
            return TryGetFacetValue(typeUsage, "Precision", false, out precision);
        }

        public static bool TryGetScale(TypeUsage typeUsage, out byte scale)
        {
            return TryGetFacetValue(typeUsage, "Scale", false, out scale);
        }

        public static bool TryGetIsUnicode(TypeUsage typeUsage, out bool isUnicode)
        {
            return TryGetFacetValue(typeUsage, "Unicode", false, out isUnicode);
        }

        private static bool TryGetFacetValue<T>(
            TypeUsage typeUsage,
            string facetDescription,
            bool checkUnbound,
            out T value)
        {
            Facet facet;

            if (!typeUsage.Facets.TryGetValue(facetDescription, false, out facet))
            {
                value = default(T);
                return false;
            }

            if (checkUnbound && facet.IsUnbounded)
            {
                value = default(T);
                return false;
            }

            if (facet.Value == null)
            {
                value = default(T);
                return false;
            }

            value = (T)facet.Value;
            return true;
        }
    }
}
