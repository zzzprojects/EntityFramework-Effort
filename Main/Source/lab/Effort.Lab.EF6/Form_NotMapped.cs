using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NMemory.StoredProcedures;

namespace Effort.Lab.EF6
{
	public partial class Form_NotMapped : Form
	{
		public Form_NotMapped()
		{
			InitializeComponent();

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
				context.EntitySimples.Add(new EntitySimple { ColumnInt = 1, Computed = int.MaxValue });
				context.EntitySimples.Add(new EntitySimple { ColumnInt = 2, LastModified = DateTime.Now });
				context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
				context.SaveChanges();
			}

			// TEST
			using (var context = new EntityContext(connection))
			{
				var list = context.EntitySimples.ToList();
				list.ForEach(x => x.LastModified = DateTime.Now);
				list.ForEach(x => x.ColumnInt = 35);
				context.SaveChanges();
			}

			using (var context = new EntityContext(connection))
			{
				var list = context.EntitySimples.ToList(); 
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

			[Column, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
			public DateTime?  LastModified { get; set; }
			[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
			public int Computed { get; set; }
		}
	}
}