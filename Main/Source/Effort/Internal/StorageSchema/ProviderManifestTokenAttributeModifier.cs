namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class ProviderManifestTokenAttributeModifier : IAttributeModifier
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

            IProviderInformation newProvider = context.Get<IProviderInformation>(ModificationContextHelper.NewProvider, null);

            if (newProvider == null)
            {
                throw new InvalidOperationException();
            }

            attribute.Value = newProvider.ManifestToken;
        }
    }
}
