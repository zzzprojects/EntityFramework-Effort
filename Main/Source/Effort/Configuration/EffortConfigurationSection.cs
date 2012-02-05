#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Effort.Configuration
{

	public class EffortConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("defaultCacheMode", DefaultValue = "true", IsRequired = false)]
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

		[ConfigurationProperty("tables")]
		[ConfigurationCollection(typeof(TableElement))]
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

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TableElement)element).Name;
		}
	}

	public class TableElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		[StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
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


		[ConfigurationProperty("cached", DefaultValue = "true", IsRequired = true)]
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
