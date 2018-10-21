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
using Effort.DataLoaders;

namespace Effort.Lab.EF6
{
    public partial class Form_General_Mik : Form
    {
        public Form_General_Mik()
        {
            InitializeComponent();


            {
                var data = new ObjectData();

                // with [Table("TableTest")] on EntitySimple ==> not insert because not EntitySimples.
                // but without  [Table("TableTest")] on EntitySimple ==> insert because name is EntitySimples.
                data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ID = 1, ColumnInt = 55 });
                data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ID = 2, ColumnInt = 55 });
                data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ID = 3, ColumnInt = 55 });


                data.Table<EntitySimple>().Add(new EntitySimple { ID = 1, ColumnInt = 55 });
                data.Table<EntitySimple>().Add(new EntitySimple { ID = 2, ColumnInt = 55 });
                data.Table<EntitySimple>().Add(new EntitySimple { ID = 3, ColumnInt = 55 });


                //data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ColumnInt = 55 });
                //data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ColumnInt = 55 });
                //data.Table<EntitySimple>("EntitySimples").Add(new EntitySimple { ColumnInt = 55 });
                data.Table<EntitySimple2>().Add(new EntitySimple2 { ColumnInt = 55 });
                data.Table<Entity>().Add(new Entity { ColumnInt = 55 });


                data.Table<Customer>().Add(new Customer { ColumnInt = 55 });
                //     data.Table<Entity>().Add(new Entity { ColumnInt = 55 });
                var dataLoader = new ObjectDataLoader(data);

                var connection = Effort.DbConnectionFactory.CreateTransient(dataLoader);

                //  var connection = Effort.DbConnectionFactory.CreateTransient();


                // SEED
                using (var context = new EntityContext(connection))
                {
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 2 });
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
                    context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 1 });
                    context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 2 });
                    context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 3 });
                    context.Entities.Add(new Entity { ColumnInt = 1 });
                    context.Entities.Add(new Entity { ColumnInt = 2 });
                    context.Entities.Add(new Entity { ColumnInt = 3 });
                    context.Customers.Add(new Customer() { ColumnInt = 1 });
                    context.Customers.Add(new Customer { ColumnInt = 2 });
                    context.Customers.Add(new Customer { ColumnInt = 3 });
                    context.SaveChanges();
                }

                // TEST
                using (var context = new EntityContext(connection))
                {
                    var list1 = context.EntitySimples.ToList(); // 6 
                    var list2 = context.EntitySimples2.ToList(); //4 
                    var list3 = context.Entities.ToList(); // need 4
                    var list4 = context.Customers.ToList(); // need 4
                }
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<EntitySimple> EntitySimples { get; set; }
            public DbSet<EntitySimple2> EntitySimples2 { get; set; }
            public DbSet<Entity> Entities { get; set; }
            public DbSet<Customer> Customers { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }
        }

        [Table("TableTest")]
        public class EntitySimple
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }

        [Table("Paul")]
        public class Customer
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }

        public class EntitySimple2
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }
        public class Entity
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }
    }
}