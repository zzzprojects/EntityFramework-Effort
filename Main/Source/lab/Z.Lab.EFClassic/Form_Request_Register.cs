using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using Effort;
using Z.EntityFramework.Classic;


namespace Z.Lab.EFClassic
{
    public partial class Form_Request_Register : Form
    {
        public Form_Request_Register()
        {
            InitializeComponent();

            var connection = DbConnectionFactory.CreateTransient();
            EntityFrameworkManager.ConfigSectionName = "entityFrameworkClassic";

            using (var context = new EntityContext(connection))
            {
                var list = new List<Customer>();
                for (var i = 0; i < 10; i++)
                {
                    list.Add(new Customer { Name = "ZZZ_" + i });
                }

                context.Customers.AddRange(list);
                context.SaveChanges();
            }

            using (var context = new EntityContext(connection))
            {
                var list = context.Customers.Where(x => x.ID > 3).ToList();
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, false)
            {
            }

            public DbSet<Customer> Customers { get; set; }
        }

        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}