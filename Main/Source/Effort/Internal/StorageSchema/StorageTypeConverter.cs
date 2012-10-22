namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Metadata.Edm;

    internal class StorageTypeConverter
    {
        private IProviderInformation oldProvider;
        private IProviderInformation newProvider;

        private IDictionary<string, PrimitiveType> oldStoreTypes;

        public StorageTypeConverter(IProviderInformation oldProvider, IProviderInformation newProvider)
        {
            this.oldProvider = oldProvider;
            this.newProvider = newProvider;

            this.oldStoreTypes = this.oldProvider.Manifest.GetStoreTypes().ToDictionary(p => p.Name);
        }

        public bool TryConvertType(string oldStorageTypeName, out string result, out Facet[] facets)
        {
            PrimitiveType oldStorageType = null;

            if (this.oldStoreTypes.TryGetValue(oldStorageTypeName, out oldStorageType))
            {
                TypeUsage oldStorageTypeUsage = TypeUsage.CreateDefaultTypeUsage(oldStorageType);
                facets = oldStorageTypeUsage.Facets.ToArray();

                // TODO: Add injection point
                if (oldStorageType.NamespaceName == "SqlServer" &&
                    (oldStorageType.Name == "timestamp" || oldStorageType.Name == "rowversion"))
                {
                    result = "rowversion";
                    return true;
                }
                else
                {
                    TypeUsage edmType = this.oldProvider.Manifest.GetEdmType(oldStorageTypeUsage);

                  
                    TypeUsage newStorageType = this.newProvider.Manifest.GetStoreType(edmType);

                    result = newStorageType.EdmType.Name;
                    return true;
                }
            }

            facets = new Facet[0];
            result = string.Empty;
            return false;
        }
    }
}
