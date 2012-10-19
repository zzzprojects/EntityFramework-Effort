namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    internal class StorageSchemaContentNameProvider
    {
        private XNamespace ns;

        public StorageSchemaContentNameProvider(XNamespace ns)
        {
            this.ns = ns;
        }

        public XName SchemaElement
        {
            get { return this.ns + "Schema"; }
        }

        public XName EntityTypeElement
        {
            get { return this.ns + "EntityType"; }
        }

        public XName FunctionElement
        {
            get { return this.ns + "Function"; }
        }

        public XName PropertyElement
        {
            get { return this.ns + "Property"; }
        }

        public XName ParameterElement
        {
            get { return this.ns + "Parameter"; }
        }

        public XName ReturnTypeElement
        {
            get { return this.ns + "ReturnType"; }
        }

        public XName CollectionTypeElement
        {
            get { return this.ns + "CollectionType"; }
        }

        public XName RowTypeElement
        {
            get { return this.ns + "RowType"; }
        }

        public XName TypeAttribute
        {
            get { return "Type"; }
        }

        public XName ReturnTypeAttribute
        {
            get { return "ReturnType"; }
        }

        public XName ProviderAttribute
        {
            get { return "Provider"; }
        }

        public XName ProviderManifestTokenAttribute
        {
            get { return "ProviderManifestToken"; }
        }
    }
}
