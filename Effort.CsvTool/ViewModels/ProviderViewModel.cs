using System;
using System.Data.Common;
using System.Reflection;

namespace Effort.CsvTool.ViewModels
{
    public class ProviderViewModel
    {
        private string name;
        private string type;

        private DbProviderFactory providerFactory;

        public ProviderViewModel(string name, string type)
        {
            this.name = name;
            this.type = type;
        }

        public string Name 
        { 
            get { return this.name; } 
        }


        public DbProviderFactory GetProviderFactory()
        {
            if (providerFactory == null)
            {
                Type factoryType = Type.GetType(type);

                FieldInfo instanceProvider = factoryType.GetField("Instance");

                providerFactory = instanceProvider.GetValue(null) as DbProviderFactory;
            }


            return providerFactory;
        }


    }
}
