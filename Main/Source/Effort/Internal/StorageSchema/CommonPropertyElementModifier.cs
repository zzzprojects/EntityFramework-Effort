namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Effort.Internal.Common.XmlProcessing;
    using System.Xml.Linq;
    using System.Data.Metadata.Edm;

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
    }
}
