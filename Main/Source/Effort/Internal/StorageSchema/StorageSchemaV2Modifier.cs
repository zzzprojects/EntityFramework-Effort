namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class StorageSchemaV2Modifier : IElementModifier
    {
        public static readonly XNamespace Namespace = StorageSchemaNamespaces.V2;

        private StorageSchemaContentNameProvider nameProvider;
        private AggregatedElementModifier modificationLogic;
        private IElementVisitor<IProviderInformation> providerParser;

        public StorageSchemaV2Modifier()
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

            // Schema.EntityType
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
