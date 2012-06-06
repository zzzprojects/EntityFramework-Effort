using System;
using System.Data.Common;

namespace Effort.Provider
{
    public class EffortConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static readonly string Key_InstanceName = "InstanceId";
        private static readonly string Key_DataLoaderType = "DataLoaderType";
        private static readonly string Key_DataLoaderArg = "DataLoaderArg";
        private static readonly string Key_DataLoaderCached = "DataLoaderCached";

        public EffortConnectionStringBuilder()
        {

        }

        public EffortConnectionStringBuilder(string connectionString)
        {
            base.ConnectionString = connectionString;
        }


        public string InstanceId
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderType))
                {
                    return string.Empty;
                }

                return base[Key_InstanceName] as string;
            }
            set
            {
                base[Key_InstanceName] = value;
            }
        }

        public Type DataProviderType
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderType))
                {
                    return null;
                }

                return Type.GetType(base[Key_DataLoaderType] as string);
            }
            set
            {
                if (value == null)
                {
                    base[Key_DataLoaderType] = null;
                    return;
                }

                base[Key_DataLoaderType] = value.FullName;
            }
        }

        public string DataProviderArg
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderType))
                {
                    return string.Empty;
                }

                return base[Key_DataLoaderArg] as string;
            }
            set
            {
                base[Key_DataLoaderArg] = value;
            }
        }

        public bool DataProviderCached
        {
            get
            {
                if (!base.ContainsKey(Key_DataLoaderCached))
                {
                    return false;
                }

                return bool.Parse(base[Key_DataLoaderCached] as string);
            }
            set
            {
                base[Key_DataLoaderArg] = value.ToString();
            }
        }
    }
}
