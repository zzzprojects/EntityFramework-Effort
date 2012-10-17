namespace Effort.Internal.StorageSchema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Effort.Internal.Common.XmlProcessing;

    internal class ModificationContextHelper
    {
        public static readonly string OriginalProvider = "OriginalProvider";

        public static readonly string NewProvider = "NewProvider";

        public static readonly string TypeConverter = "TypeConverter";

        public static StorageTypeConverter GetTypeConverter(IModificationContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("context");
            }

            StorageTypeConverter converter = context.Get<StorageTypeConverter>(TypeConverter, null);

            if (converter != null)
            {
                return converter;
            }

            // Create the converter
            // Get the provider informations first
            IProviderInformation originalProvider =
                context.Get<IProviderInformation>(ModificationContextHelper.OriginalProvider, null);

            IProviderInformation newProvider =
                context.Get<IProviderInformation>(ModificationContextHelper.NewProvider, null);

            if (originalProvider == null)
            {
                throw new ArgumentException("", "context");
            }

            if (newProvider == null)
            {
                throw new ArgumentException("", "context");
            }

            converter = new StorageTypeConverter(originalProvider, newProvider);

            // Store for future usage
            context.Set(TypeConverter, converter);
            
            return converter;
        }
    }
}
