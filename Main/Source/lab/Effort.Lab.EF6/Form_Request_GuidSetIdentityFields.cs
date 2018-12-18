using Effort.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
	public partial class Form_Request_GuidSetIdentityFields : Form
	{
		public Form_Request_GuidSetIdentityFields()
		{
			InitializeComponent();

			var connection = (EffortConnection)DbConnectionFactory.CreateTransient();

			// MUST initialize first with the context with identity (Required for SetIdentityFields(false)
			using (var context = new EntityContext(connection))
			{
				context.Database.CreateIfNotExists();
			}

			// MUST open connection first (Required for SetIdentityFields(false)
			connection.Open();
			connection.DbManager.SetIdentityFields(false);
			connection.Close();

			// SEED
			using (var context = new EntityContextNoIdentity(connection))
			{
				//context.EntitySimples.Add(new EntitySimple { Id = 10, ColumnInt = 1 });
				//context.EntitySimples.Add(new EntitySimple { Id = 11, ColumnInt = 2 });
				//context.EntitySimples.Add(new EntitySimple { Id = 12, ColumnInt = 3 });



				context.EntitySimples.Add(new EntitySimple { Id = new Guid("0FA0F3D7-57CB-42BB-819A-BB9C4D0645CC"), ColumnInt = 1 });
				context.EntitySimples.Add(new EntitySimple { Id = new Guid("C3E9517D-B444-4F56-B459-5B9804C03FE1"), ColumnInt = 2 });
				context.EntitySimples.Add(new EntitySimple { Id = new Guid("A02E175C-C050-4996-AD42-B722BE1E7FF1"), ColumnInt = 3 });
				context.SaveChanges();
			}

			// MUST open connection first (Required for SetIdentityFields(false)
			connection.Open();
			connection.DbManager.SetIdentityFields(true);
			connection.Close();

			// TEST
			using (var context = new EntityContext(connection))
			{
				context.EntitySimples.Add(new EntitySimple() { ColumnInt = 4 });
				context.SaveChanges();
				var list = context.EntitySimples.ToList();
			}
		}

		public class EntityContext : DbContext
		{
			public EntityContext(DbConnection connection) : base(connection, true)
			{
			}

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				modelBuilder.Entity<EntitySimple>().HasKey(x => x.Id);
				modelBuilder.Entity<EntitySimple>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

				base.OnModelCreating(modelBuilder);
			}

			public DbSet<EntitySimple> EntitySimples { get; set; }
		}

		public class EntityContextNoIdentity : EntityContext
		{
			public EntityContextNoIdentity(DbConnection connection) : base(connection)
			{
			}

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				modelBuilder.Entity<EntitySimple>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			}
		}

		public class EntitySimple
		{
			public Guid Id { get; set; }
			//public int Id { get; set; }

			public int ColumnInt { get; set; }
		}

	}
}