// --------------------------------------------------------------------------------------------
// <copyright file="StorageSchemaV1Modifier.cs" company="Effort Team">
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

    internal class StorageSchemaV1Modifier : IElementModifier
    {
        public static readonly XNamespace Namespace = StorageSchemaNamespaces.V1;

        private StorageSchemaContentNameProvider nameProvider;
        private AggregatedElementModifier modificationLogic;
        private IElementVisitor<IProviderInformation> providerParser;

        public StorageSchemaV1Modifier()
        {
            this.nameProvider = new StorageSchemaContentNameProvider(Namespace);

            this.providerParser = new ProviderParser(this.nameProvider);

            this.modificationLogic = new AggregatedElementModifier();

            // Schema[Provider] : Provider
            this.modificationLogic.AddModifier(
                new ComposedElementModifier(
                    new SelfElementSelector(),
                    new ProviderAttributeSelector(this.nameProvider),
                    new ProviderAttributeModifier()));

            // Schema[ProviderManifestToken] : ProviderManifestToken
            this.modificationLogic.AddModifier(
                new ComposedElementModifier(
                    new SelfElementSelector(),
                    new ProviderManifestTokenAttributeSelector(this.nameProvider),
                    new ProviderManifestTokenAttributeModifier()));

            // Schema.EntityType[Type] : PropertyType
            this.modificationLogic.AddModifier(
                new ComposedElementModifier(
                    new EntityTypePropertyElementSelector(this.nameProvider),
                    new CommonPropertyElementModifier(this.nameProvider)));

            // Schema.Function.Parameter[Type] : FunctionType
            this.modificationLogic.AddModifier(
                new ComposedElementModifier(
                    new FunctionParameterElementSelector(this.nameProvider),
                    new TypeAttributeSelector(this.nameProvider),
                    new FunctionTypeAttributeModifier()));

            // Schema.Function[ReturnType] : FunctionType
            this.modificationLogic.AddModifier(
                new ComposedElementModifier(
                    new FunctionElementSelector(this.nameProvider),
                    new ReturnTypeAttributeSelector(this.nameProvider),
                    new FunctionTypeAttributeModifier()));
        }

        public void Modify(XElement ssdl, IModificationContext context)
        {
            if (ssdl == null)
            {
                throw new ArgumentNullException("ssdl");
            }

            if (ssdl.Name != this.nameProvider.SchemaElement)
            {
                throw new ArgumentException("", "ssdl");
            }

            // Parse and store original provider information
            IProviderInformation providerInfo = this.providerParser.VisitElement(ssdl);
            context.Set(ModificationContextHelper.OriginalProvider, providerInfo);   

            // Modify the xml
            this.modificationLogic.Modify(ssdl, context);
        }
    }
}
