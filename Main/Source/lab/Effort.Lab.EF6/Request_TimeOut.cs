using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effort.Lab.EF6
{
	public class Request_TimeOut
	{
		public static void Execute()
		{
			var connection = Effort.DbConnectionFactory.CreateTransient();

			// CLEAN
			using (var context = new EntityContext(connection))
			{
				context.EntitySimples.RemoveRange(context.EntitySimples);
				context.SaveChanges();
			}

			// SEED
			using (var context = new EntityContext(connection))
			{
				connection.SetConnectionTimeout(500000); 
				
				var list = new List<EntitySimple>();
				for (int i = 0; i < 1000000; i++)
				{
					list.Add(new EntitySimple() {ColumnInt = i});
				}

				context.EntitySimples.AddRange(list);
				context.SaveChanges();
			}
			 
		}

		public class EntityContext : DbContext
		{
			public EntityContext(DbConnection connection) : base(connection, true)
			{
			}

			public DbSet<EntitySimple> EntitySimples { get; set; }


			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
			}
		}

		public class EntitySimple
		{
			public int ID { get; set; }
			public int ColumnInt { get; set; }
		}
	} 
}

