using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Effort.Extra;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_EffortExtra : Form
    {
        public Form_Request_EffortExtra()
        {
            InitializeComponent();

            var data = new ObjectData(TableNamingStrategy.Pluralised);

            var entitySimples = data.Table<EntitySimple>();
            entitySimples.Add(new EntitySimple { ID = 1, ColumnInt = -1 });
            entitySimples.Add(new EntitySimple { ID = 2, ColumnInt = -2 });

            var objectDataLoader = new ObjectDataLoader(data);

            var connection = Effort.DbConnectionFactory.CreateTransient(objectDataLoader);
    
            //// CLEAN
            //using (var context = new EntityContext(connection))
            //{
            //    context.EntitySimples.RemoveRange(context.EntitySimples);
            //    context.SaveChanges();
            //}

            // SEED
            using (var context = new EntityContext(connection))
            {
                context.Database.CreateIfNotExists();
                connection.CreateRestorePoint();


                context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                var list = context.EntitySimples.ToList();
                context.SaveChanges();
                connection.CreateRestorePoint();

                context.EntitySimples.Add(new EntitySimple { ColumnInt = 2 });
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
                context.SaveChanges();

                connection.RollbackToRestorePoint();
            }

            // TEST
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
        }
    }
}
