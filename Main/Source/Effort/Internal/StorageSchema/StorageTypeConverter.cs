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

        public bool TryConvertType(string oldStorageTypeName, out string result)
        {
            IDictionary<string, PrimitiveType> oldStoreTypes =
                this.oldProvider.Manifest.GetStoreTypes().ToDictionary(p => p.Name);

            PrimitiveType oldStorageType = null;

            if (oldStoreTypes.TryGetValue(oldStorageTypeName, out oldStorageType))
            {
                // TODO: Add injection point
                if (oldStorageType.NamespaceName == "SqlServer" &&
                    (oldStorageType.Name == "timestamp" || oldStorageType.Name == "rowversion"))
                {
                    result = "rowversion";
                    return true;
                }
                else
                {
                    TypeUsage edmType = 
                        this.oldProvider.Manifest.GetEdmType(
                            TypeUsage.CreateDefaultTypeUsage(oldStorageType));

                    TypeUsage newStorageType = this.newProvider.Manifest.GetStoreType(edmType);

                    result = newStorageType.EdmType.Name;
                    return true;
                }
            }

            result = string.Empty;
            return false;
        }
    }
}
