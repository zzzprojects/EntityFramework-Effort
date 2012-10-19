namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Effort.Internal.Common.XmlProcessing;

    internal class UniversalStorageSchemaModifier
    {
        private IElementModifier schemaV1Modifier;
        private IElementModifier schemaV2Modifier;
        private IElementModifier schemaV3Modifier;

        public static readonly UniversalStorageSchemaModifier Instance = new UniversalStorageSchemaModifier();

        public void Modify(XElement ssdl, IProviderInformation newProvider)
        {
            if (ssdl == null)
            {
                throw new ArgumentNullException("root");
            }

            if (newProvider == null)
            {
                throw new ArgumentNullException("newProvider");
            }

            IElementModifier appropriateModifier = null;
            XNamespace ns = ssdl.Name.Namespace;

            // Find the appropriate modifier
            if (ns == StorageSchemaV1Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV1Modifier;
            }
            else if (ns == StorageSchemaV2Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV2Modifier;
            }
            else if (ns == StorageSchemaV3Modifier.Namespace)
            {
                appropriateModifier = this.SchemaV3Modifier;
            }

            if (appropriateModifier == null)
            {
                throw new ArgumentException("", "root");
            }

            IModificationContext context = new ModificationContext();
            context.Set(ModificationContextHelper.NewProvider, newProvider);

            appropriateModifier.Modify(ssdl, context);
        }

        protected IElementModifier SchemaV1Modifier
        {
            get
            {
                if (this.schemaV1Modifier == null)
                {
                    this.schemaV1Modifier = new StorageSchemaV1Modifier();
                }

                return this.schemaV1Modifier;
            }
        }

        protected IElementModifier SchemaV2Modifier
        {
            get
            {
                if (this.schemaV2Modifier == null)
                {
                    this.schemaV2Modifier = new StorageSchemaV2Modifier();
                }

                return this.schemaV2Modifier;
            }
        }

        protected IElementModifier SchemaV3Modifier
        {
            get
            {
                if (this.schemaV3Modifier == null)
                {
                    this.schemaV3Modifier = new StorageSchemaV3Modifier();
                }

                return this.schemaV3Modifier;
            }
        }
    }
}
