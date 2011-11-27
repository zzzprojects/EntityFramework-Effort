using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Effort.CodeFirst.Test
{
	public class ProductContext : DbContext
	{
		public IDbSet<Category> Categories { get; set; }
		public IDbSet<Product> Products { get; set; }

		public ProductContext()
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		//public override int SaveChanges()
		//{
		//    this.ChangeTracker.DetectChanges();
		//    foreach (var e in this.ChangeTracker.Entries())
		//    {
		//    }
		//    return base.SaveChanges();
		//}

		protected override bool ShouldValidateEntity(System.Data.Entity.Infrastructure.DbEntityEntry entityEntry)
		{
			return base.ShouldValidateEntity(entityEntry);

		}

		protected override System.Data.Entity.Validation.DbEntityValidationResult ValidateEntity(System.Data.Entity.Infrastructure.DbEntityEntry entityEntry, IDictionary<object, object> items)
		{
			return base.ValidateEntity(entityEntry, items);
		}
	}
}
