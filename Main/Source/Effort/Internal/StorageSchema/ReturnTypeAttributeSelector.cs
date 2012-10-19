namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class ReturnTypeAttributeSelector : IElementAttributeSelector
    {
        private StorageSchemaContentNameProvider nameProvider;

        public ReturnTypeAttributeSelector(StorageSchemaContentNameProvider nameProvider)
        {
            this.nameProvider = nameProvider;
        }

        public XAttribute SelectAttribute(XElement element)
        {
            if (element == null)
	        {
		        throw new ArgumentNullException("element");
	        }

            return element.Attribute(this.nameProvider.ReturnTypeAttribute);
        }
    }
}
