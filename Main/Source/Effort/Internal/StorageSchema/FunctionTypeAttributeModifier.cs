namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;
    using System.Data.Metadata.Edm;

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
