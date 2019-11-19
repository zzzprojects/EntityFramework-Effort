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
    public partial class Form_Request_DisableFK : Form
    {
        public Form_Request_DisableFK()
        {
            InitializeComponent();


            var connection = Effort.DbConnectionFactory.CreateTransient();
            // SEED
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 1 });
                context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 2 });
                context.EntitySimples2.Add(new EntitySimple2 { ColumnInt = 3 });
                context.Entities.Add(new Entity { ColumnInt = 1 });
                context.Entities.Add(new Entity { ColumnInt = 2 });
                context.Entities.Add(new Entity { ColumnInt = 3 });

                context.SaveChanges();
            }

            // TEST
            using (var context = new EntityContext(connection))
            {
                var list2 = context.EntitySimples2.ToList();
                var list3 = context.Entities.ToList();
            }

        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<EntitySimple2> EntitySimples2 { get; set; }
            public DbSet<Entity> Entities { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }
        }

        public class EntitySimple2
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }

            public int EntityId { get; set; }
        }

        public class Entity
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }

            public ICollection<EntitySimple2> EntitySimple2s { get; set; }

        }
    }
}