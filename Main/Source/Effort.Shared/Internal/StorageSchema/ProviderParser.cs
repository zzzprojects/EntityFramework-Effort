// --------------------------------------------------------------------------------------------
// <copyright file="ProviderParser.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class ProviderParser : IElementVisitor<IProviderInformation>
    {
        private StorageSchemaContentNameProvider nameProvider;

        public ProviderParser(StorageSchemaContentNameProvider nameProvider)
        {
            this.nameProvider = nameProvider;
        }

        public IProviderInformation VisitElement(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Name != this.nameProvider.SchemaElement)
            {
                throw new ArgumentException("", "element");
            }

            XAttribute provider = element.Attribute(this.nameProvider.ProviderAttribute);
            XAttribute providerManifestToken = element.Attribute(this.nameProvider.ProviderManifestTokenAttribute);

            IProviderInformation providerInformation = 
                new ProviderInformation(
                    provider.Value, 
                    providerManifestToken.Value);

            return providerInformation;
        }
    }
}
