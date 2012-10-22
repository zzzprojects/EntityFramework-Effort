namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class EntityTypePropertyElementSelector : IElementSelector
    {
        private StorageSchemaContentNameProvider nameProvider;

        public EntityTypePropertyElementSelector(StorageSchemaContentNameProvider nameProvider)
        {
            if (nameProvider == null)
            {
                throw new ArgumentNullException("nameProvider");
            }

            this.nameProvider = nameProvider;
        }

        public IEnumerable<XElement> SelectElements(XElement root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (root.Name != this.nameProvider.SchemaElement)
            {
                throw new ArgumentException("", "root");
            }

            return root
                .Elements(this.nameProvider.EntityTypeElement)
                .SelectMany(x => x.Elements(this.nameProvider.PropertyElement));
        }
    }
}
