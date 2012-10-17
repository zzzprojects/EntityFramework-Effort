namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class PropertyTypeAttributeModifier : IAttributeModifier
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

            string newStorageType = null;

            if (converter.TryConvertType(oldStorageType, out newStorageType))
            {
                attribute.Value = newStorageType;
            }
        }
    }
}
