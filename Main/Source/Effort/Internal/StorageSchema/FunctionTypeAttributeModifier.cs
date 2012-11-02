// ----------------------------------------------------------------------------------
// <copyright file="FunctionTypeAttributeModifier.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class FunctionTypeAttributeModifier : IAttributeModifier
    {
        public void Modify(XAttribute attribute, IModificationContext context)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            string oldStorageType = attribute.Value;
            StorageTypeConverter converter = ModificationContextHelper.GetTypeConverter(context);

            // Try to get the collection value
            string pattern = @"Collection\(([^ \t]{1,}(?:\.[^ \\t]{1,}){0,})\)";
            Match regex = Regex.Match(oldStorageType, pattern);
            bool isCollection = regex.Success;

            // If it was a collection type, get the inside value
            if (isCollection)
            {
                oldStorageType = regex.Groups[1].Value;
            }

            string newStorageType = null;

            Facet[] facets = null;

            if (converter.TryConvertType(oldStorageType, out newStorageType, out facets))
            {
                if (isCollection)
                {
                    attribute.Value = string.Format("Collection({0})", newStorageType);
                }
                else
                {
                    attribute.Value = newStorageType;
                }
            }
        }
    }
}
