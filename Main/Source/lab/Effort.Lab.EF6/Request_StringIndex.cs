using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effort.Lab.EF6
{
	public class Request_StringIndex
	{
		public static void Execute()
		{
			var connection = DbConnectionFactory.CreateTransient();

			using (var context = new EntityContext(connection))
			{
				context.EntitySimples.Add(new EntitySimple() { Name = "test1", ColumnInt = 1 });
				context.EntitySimples.Add(new EntitySimple() { Name = "test2", ColumnInt = 2 });
				context.BulkSaveChanges();
			}


			using (var context = new EntityContext(connection))
			{
				List<EntitySimple> list = new List<EntitySimple>();

				list.Add(new EntitySimple() { Name = "test1", ColumnInt = 50 });
				list.Add(new EntitySimple() { Name = "test3", ColumnInt = 50 });

				context.BulkMerge(list);
			}



			using (var context = new EntityContext(connection))
			{
				var list = context.EntitySimples.ToList();
			}




			using (var context = new EntityContext(connection))
			{
				context.EntityIndexStrings.Add(new EntityIndexString() { Name = "test1", ColumnInt = 1 });
				context.EntityIndexStrings.Add(new EntityIndexString() { Name = "test2", ColumnInt = 2 });
				context.BulkSaveChanges();
			}


			using (var context = new EntityContext(connection))
			{
				var list = context.EntityIndexStrings.Where(x => x.Name == "test1").ToList();

				list.Add(new EntityIndexString() { Name = "test3", ColumnInt = 3 });

				list.ForEach(x => x.ColumnInt += 200);

				context.BulkMerge(list);
			}



			using (var context = new EntityContext(connection))
			{
				var list = context.EntityIndexStrings.ToList();
			}
		}
	}

	public class EntityContext : DbContext
	{
		public EntityContext(DbConnection connection) : base(connection, true)
		{
		}

		public DbSet<EntitySimple> EntitySimples { get; set; }
		public DbSet<EntityIndexString> EntityIndexStrings { get; set; }
	}

	public class EntitySimple
	{
		[Key]
		public string Name { get; set; }
		public int ColumnInt { get; set; }
	}

	public class EntityIndexString
	{
		[Key]
		public Int64 Id { get; set; }

		[Index(IsUnique = true)]
		public string Name { get; set; }
		public int ColumnInt { get; set; }
	}
}