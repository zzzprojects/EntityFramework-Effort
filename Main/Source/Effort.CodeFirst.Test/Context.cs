using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Effort.CodeFirst.Test
{
	public class NorthwindContext : DbContext
	{
		public IDbSet<Category> Categories { get; set; }
		public IDbSet<Product> Products { get; set; }
	}
}
