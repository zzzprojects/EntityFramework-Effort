using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Reflection;

namespace Effort.Provider
{
    public class EffortConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static readonly string Key_InstanceName = "InstanceId";
        private static readonly string Key_DataProviderType = "DataSourceType";
        private static readonly string Key_DataProviderArg = "DataSourceArg";

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
                if (!base.ContainsKey(Key_DataProviderType))
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
                if (!base.ContainsKey(Key_DataProviderType))
                {
                    return null;
                }

                return Type.GetType(base[Key_DataProviderType] as string);
            }
            set
            {
                if (value == null)
                {
                    base[Key_DataProviderType] = null;
                    return;
                }

                base[Key_DataProviderType] = value.FullName;
            }
        }

        public string DataProviderArg
        {
            get
            {
                if (!base.ContainsKey(Key_DataProviderType))
                {
                    return string.Empty;
                }

                return base[Key_DataProviderArg] as string;
            }
            set
            {
                base[Key_DataProviderArg] = value;
            }
        }
    }
}
