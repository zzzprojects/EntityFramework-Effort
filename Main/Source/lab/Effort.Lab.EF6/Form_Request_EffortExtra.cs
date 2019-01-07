using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Effort.DataLoaders;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_EffortExtra : Form
    {
        public Form_Request_EffortExtra()
        {
            InitializeComponent();

            var data = new ObjectData();

            var entitySimples = data.Table<EntitySimple>();
            entitySimples.Add(new EntitySimple {ID = 1, ColumnInt = -1});
            entitySimples.Add(new EntitySimple {ID = 2, ColumnInt = -2});

            var objectDataLoader = new ObjectDataLoader(data);

            var connection = DbConnectionFactory.CreateTransient(objectDataLoader);

            // SEED
            using (var context = new EntityContext(connection))
            {
                context.Database.CreateIfNotExists();

                context.EntitySimples.Add(new EntitySimple {ColumnInt = 1});
                var list = context.EntitySimples.ToList();
                context.SaveChanges();
                connection.CreateRestorePoint();

                context.EntitySimples.Add(new EntitySimple {ColumnInt = 2});
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 3});
                context.SaveChanges();
                var list2 = context.EntitySimples.ToList();
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
            public DbSet<AnotherTable> AnotherTables { get; set; }

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

        public class AnotherTable
        {
            public int ID { get; set; }

            [Index]
            public int ColumnInt { get; set; }
        }
    }
}