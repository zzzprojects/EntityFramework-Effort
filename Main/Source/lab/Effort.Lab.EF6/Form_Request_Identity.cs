using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Effort.Provider;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_Identity : Form
    {
        public Form_Request_Identity()
        {
            InitializeComponent();

            var connection = (EffortConnection) DbConnectionFactory.CreateTransient();

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
                context.EntitySimples.Add(new EntitySimple {ID = 4, ColumnInt = 1});
                context.EntitySimples.Add(new EntitySimple {ID = 12, ColumnInt = 2});
                context.EntitySimples.Add(new EntitySimple {ID = 24, ColumnInt = 3});
                context.SaveChanges();
            }

            // MUST open connection first (Required for SetIdentityFields(false)
            connection.Open();
            connection.DbManager.SetIdentityFields(true);
            connection.Close();

            // TEST
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples.Add(new EntitySimple() {ColumnInt = 4});
                context.SaveChanges();
                var list = context.EntitySimples.ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
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
                modelBuilder.Entity<EntitySimple>().Property(x => x.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                base.OnModelCreating(modelBuilder);
            }
        }

        public class EntitySimple
        {
            [Key]
            public int ID { get; set; }

            public int ColumnInt { get; set; }
        }
    }
}