
namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
