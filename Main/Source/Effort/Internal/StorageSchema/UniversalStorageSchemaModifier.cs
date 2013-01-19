// --------------------------------------------------------------------------------------------
// <copyright file="UniversalStorageSchemaModifier.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

    internal class UniversalStorageSchemaModifier
    {
        public static readonly UniversalStorageSchemaModifier Instance = 
            new UniversalStorageSchemaModifier();

        private IElementModifier schemaV1Modifier;
        private IElementModifier schemaV2Modifier;
        private IElementModifier schemaV3Modifier;

        public void Modify(XElement ssdl, IProviderInformation newProvider)
        {
            if (ssdl == null)
            {
                throw new ArgumentNullException("root");
            }

            if (newProvider == null)
            {
                throw new ArgumentNullException("newProvider");
            }

            IElementModifier appropriateModifier = null;
            XNamespace ns = ssdl.Name.Namespace;

            // Find the appropriate modifier
            if (ns == StorageSchemaV1Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV1Modifier;
            }
            else if (ns == StorageSchemaV2Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV2Modifier;
            }
            else if (ns == StorageSchemaV3Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV3Modifier;
            }

            if (appropriateModifier == null)
            {
                throw new ArgumentException("", "root");
            }

            IModificationContext context = new ModificationContext();
            context.Set(ModificationContextHelper.NewProvider, newProvider);

            appropriateModifier.Modify(ssdl, context);
        }

        protected IElementModifier SchemaV1Modifier
        {
            get
            {
                if (this.schemaV1Modifier == null)
                {
                    this.schemaV1Modifier = new StorageSchemaV1Modifier();
                }

                return this.schemaV1Modifier;
            }
        }

        protected IElementModifier SchemaV2Modifier
        {
            get
            {
                if (this.schemaV2Modifier == null)
                {
                    this.schemaV2Modifier = new StorageSchemaV2Modifier();
                }

                return this.schemaV2Modifier;
            }
        }

        protected IElementModifier SchemaV3Modifier
        {
            get
            {
                if (this.schemaV3Modifier == null)
                {
                    this.schemaV3Modifier = new StorageSchemaV3Modifier();
                }

                return this.schemaV3Modifier;
            }
        }
    }
}
