// --------------------------------------------------------------------------------------------
// <copyright file="StorageSchemaContentNameProvider.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using System.Xml.Linq;

    internal class StorageSchemaContentNameProvider
    {
        private XNamespace ns;

        public StorageSchemaContentNameProvider(XNamespace ns)
        {
            this.ns = ns;
        }

        public XName SchemaElement
        {
            get { return this.ns + "Schema"; }
        }

        public XName EntityTypeElement
        {
            get { return this.ns + "EntityType"; }
        }

        public XName FunctionElement
        {
            get { return this.ns + "Function"; }
        }

        public XName PropertyElement
        {
            get { return this.ns + "Property"; }
        }

        public XName ParameterElement
        {
            get { return this.ns + "Parameter"; }
        }

        public XName ReturnTypeElement
        {
            get { return this.ns + "ReturnType"; }
        }

        public XName CollectionTypeElement
        {
            get { return this.ns + "CollectionType"; }
        }

        public XName RowTypeElement
        {
            get { return this.ns + "RowType"; }
        }

        public XName TypeAttribute
        {
            get { return "Type"; }
        }

        public XName ReturnTypeAttribute
        {
            get { return "ReturnType"; }
        }

        public XName ProviderAttribute
        {
            get { return "Provider"; }
        }

        public XName ProviderManifestTokenAttribute
        {
            get { return "ProviderManifestToken"; }
        }

        public XName MaxLengthAttribute
        {
            get { return "MaxLength"; }
        }

        public XName FixedLengthAttribute
        {
            get { return "FixedLength"; }
        }

        public XName PrecisionAttribute
        {
            get { return "Precision"; }
        }

        public XName ScaleAttribute
        {
            get { return "Scale"; }
        }

        public XName UnicodeAttribute
        {
            get { return "Unicode"; }
        }

        public XName CollationAttribute
        {
            get { return "Collation"; }
        }

        public XName NullableAttribute
        {
            get { return "Nullable"; }
        }

        public XName DefaultValueAttribute
        {
            get { return "DefaultValue"; }
        }
    }
}
