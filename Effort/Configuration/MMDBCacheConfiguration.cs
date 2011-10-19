using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Effort.Configuration
{
    public class MMDBCacheConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty( "defaultCacheMode", DefaultValue = "true", IsRequired = false )]
        public Boolean DefaultCacheMode
        {
            get
            {
                return (Boolean)this["defaultCacheMode"];
            }
            set
            {
                this["defaultCacheMode"] = value;
            }
        }

        [ConfigurationProperty( "tables" )]
        [ConfigurationCollection( typeof( TableElement ) )]
        public TableElementCollection Tables
        {
            get
            {
                return (TableElementCollection)this["tables"];
            }
            set
            {
                this["tables"] = value;
            }
        }
    }

    public class TableElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TableElement();
        }

        protected override object GetElementKey( ConfigurationElement element )
        {
            return ( (TableElement)element ).Name;
        }
    }


    // Define the "font" element
    // with "name" and "size" attributes.
    public class TableElement : ConfigurationElement
    {
        [ConfigurationProperty( "name", IsRequired = true )]
        [StringValidator( InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\" )]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }


        [ConfigurationProperty( "cached", DefaultValue = "true", IsRequired = true )]
        public Boolean Cached
        {
            get
            {
                return (Boolean)this["cached"];
            }
            set
            {
                this["cached"] = value;
            }
        }
    }
}
