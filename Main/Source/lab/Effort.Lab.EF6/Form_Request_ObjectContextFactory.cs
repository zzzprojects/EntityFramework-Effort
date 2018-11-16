using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_ObjectContextFactory : Form
    {
        public Form_Request_ObjectContextFactory()
        {
            InitializeComponent();

            var connection = DbConnectionFactory.CreateTransient();

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
                var list = context.EntitySimples.ToList();
            }
        }

        public class MyObjectContextClass : ObjectContext
        {

            public MyObjectContextClass(EntityConnection connection) : base(connection)
            {
            }

            public MyObjectContextClass(EntityConnection connection, bool contextOwnsConnection) : base(connection, contextOwnsConnection)
            {
            }

            public MyObjectContextClass(string connectionString) : base(connectionString)
            {
            }

            protected MyObjectContextClass(string connectionString, string defaultContainerName) : base(connectionString, defaultContainerName)
            {
            }

            protected MyObjectContextClass(EntityConnection connection, string defaultContainerName) : base(connection, defaultContainerName)
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
                modelBuilder.Configurations.Add(new EmailNotificationSchemeMapping());
                base.OnModelCreating(modelBuilder);
            }
        }

        public partial class EmailNotificationSchemeMapping : EntityTypeConfiguration<EntitySimple>
        {

            public EmailNotificationSchemeMapping()
            {
                this
                    .Map(tph => {
                      //  tph.Requires("Recipient").HasValue("Email");
                        tph.ToTable("NotificationScheme");
                    });
                //this.Property(p => p.EmailAddress)
                //    .IsRequired()
                //    .IsUnicode(false);
                //OnCreated();
            }

           // partial void OnCreated();

        }

        public class EntitySimple
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }
    }
}