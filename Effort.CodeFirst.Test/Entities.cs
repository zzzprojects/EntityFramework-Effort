using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Effort.CodeFirst.Test
{
	public class Category
	{
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string CategoryId { get; set; }
		public string Name { get; set; }

		public virtual ICollection<Product> Products { get; set; }
	}

	public class Product
	{
		[Key, Required, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string CategoryId { get; set; }

		public virtual Category Category { get; set; }
	}
}
