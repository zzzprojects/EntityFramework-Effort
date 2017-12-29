using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_LazyLoading : Form
    {
        public Form_Request_LazyLoading()
        {
            InitializeComponent();

            var connection = DbConnectionFactory.CreateTransient();

            // CLEAN
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples.RemoveRange(context.EntitySimples);
                context.EntitySimpleLazy.RemoveRange(context.EntitySimpleLazy);
                context.SaveChanges();
            }

            // SEED
            using (var context = new EntityContext(connection))
            {
                var entity1 = context.EntitySimples.Add(new EntitySimple {ColumnInt = 1, EntitySimpleLazy = new List<EntitySimpleLazy>()});
                entity1.EntitySimpleLazy.Add(new EntitySimpleLazy {ColumnInt = 10});
                entity1.EntitySimpleLazy.Add(new EntitySimpleLazy {ColumnInt = 20});
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 2});
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 3});
                context.SaveChanges();

                // Proxy type are not created since entities (without proxy) already exists in the context
                var list = context.EntitySimples.ToList();
            }

            // TEST
            using (var context = new EntityContext(connection))
            {
                // Proxy type are created since entities are loaded from the memory
                var list = context.EntitySimples.ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
                //this.Configuration.LazyLoadingEnabled = true;
            }

            public DbSet<EntitySimple> EntitySimples { get; set; }

            public DbSet<EntitySimpleLazy> EntitySimpleLazy { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Types().Configure(x =>
                    x.ToTable(GetType().DeclaringType != null
                        ? GetType().DeclaringType.FullName.Replace(".", "_") + "_" + x.ClrType.Name
                        : ""));

                base.OnModelCreating(modelBuilder);
            }
        }

        public class EntitySimple
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }

            public virtual ICollection<EntitySimpleLazy> EntitySimpleLazy { get; set; }
        }

        public class EntitySimpleLazy
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }
    }
}