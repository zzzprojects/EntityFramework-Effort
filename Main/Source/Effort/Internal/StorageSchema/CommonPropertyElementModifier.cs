// --------------------------------------------------------------------------------------------
// <copyright file="CommonPropertyElementModifier.cs" company="Effort Team">
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

namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class CommonPropertyElementModifier : IElementModifier
    {
        private StorageSchemaContentNameProvider nameProvider;

        public CommonPropertyElementModifier(StorageSchemaContentNameProvider nameProvider)
        {
            if (nameProvider == null)
            {
                throw new ArgumentNullException("nameProvider");
            }

            this.nameProvider = nameProvider;
        }

        private IEnumerable<XName> CommonPropertyAttributeNames
        {
            get
            {
                yield return this.nameProvider.MaxLengthAttribute;

                yield return this.nameProvider.FixedLengthAttribute;

                yield return this.nameProvider.PrecisionAttribute;

                yield return this.nameProvider.ScaleAttribute;

                yield return this.nameProvider.UnicodeAttribute;

                yield return this.nameProvider.CollationAttribute;

                yield return this.nameProvider.NullableAttribute;

                yield return this.nameProvider.DefaultValueAttribute;
            }
        }

        public void Modify(XElement element, IModificationContext context)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (element.Name != this.nameProvider.PropertyElement)
            {
                throw new ArgumentException("", "context");
            }

            StorageTypeConverter converter = ModificationContextHelper.GetTypeConverter(context);

            XAttribute typeAttribute = element.Attribute(this.nameProvider.TypeAttribute);

            Facet[] facets = null;
            string oldStorageType = typeAttribute.Value;
            string newStorageType = null;

            if (converter.TryConvertType(oldStorageType, out newStorageType, out facets))
            {
                typeAttribute.Value = newStorageType;

                foreach (XName commonAttributeName in this.CommonPropertyAttributeNames)
                {
                    if (element.Attribute(commonAttributeName) != null)
                    {
                        // Element contains the attribute
                        continue;
                    }

                    // Seach for default facet value
                    Facet facet = facets.FirstOrDefault(f => f.Name == commonAttributeName.LocalName);

                    if (facet != null && facet.Value != null)
                    {
                        element.Add(new XAttribute(commonAttributeName, facet.Value));
                    }
                }
            }
        }
    }
}
