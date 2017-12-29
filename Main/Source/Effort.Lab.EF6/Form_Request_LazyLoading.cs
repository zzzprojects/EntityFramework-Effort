using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_LazyLoading : Form
    {
        public Form_Request_LazyLoading()
        {
            InitializeComponent();
            // CLEAN
            using (var context = new EntityContext())
            {
                context.EntitySimple.RemoveRange(context.EntitySimple);
                context.SaveChanges();
            }

            // SEED
            using (var context = new EntityContext())
            {
                context.EntitySimple.Add(new EntitySimple() { ColumnInt = 1 });
                context.EntitySimple.Add(new EntitySimple() { ColumnInt = 2 });
                context.EntitySimple.Add(new EntitySimple() { ColumnInt = 3 });
                context.SaveChanges();
            }

            // TEST
            using (var context = new EntityContext())
            {


            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext() : base("CodeFirstEntities")
            {
            }

            public DbSet<EntitySimple> EntitySimple { get; set; }

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