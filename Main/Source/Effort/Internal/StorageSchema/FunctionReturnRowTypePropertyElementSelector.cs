namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class FunctionReturnRowTypePropertyElementSelector : IElementSelector
    {
        private StorageSchemaContentNameProvider nameProvider;

        public FunctionReturnRowTypePropertyElementSelector(StorageSchemaContentNameProvider nameProvider)
        {
            this.nameProvider = nameProvider;
        }

        public IEnumerable<XElement> SelectElements(XElement root)
        {
            return root
                .Elements(this.nameProvider.FunctionElement)
                .SelectMany(e =>
                    e.Elements(this.nameProvider.ReturnTypeElement))
                .SelectMany(e =>
                    e.Elements(this.nameProvider.CollectionTypeElement))
                .SelectMany(e =>
                    e.Elements(this.nameProvider.RowTypeElement))
                .SelectMany(e =>
                    e.Elements(this.nameProvider.PropertyElement));
        }
    }
}
