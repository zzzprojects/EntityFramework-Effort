using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class __Form_Template : Form
    {
        public __Form_Template()
        {
            InitializeComponent();

            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();

            // CLEAN
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples.RemoveRange(context.EntitySimples);
                context.SaveChanges();
            }

            // SEED
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 1});
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 2});
                context.EntitySimples.Add(new EntitySimple {ColumnInt = 3});
                context.SaveChanges();
            }

            // TEST
            using (var context = new EntityContext(connection))
            {
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
        }
    }
}