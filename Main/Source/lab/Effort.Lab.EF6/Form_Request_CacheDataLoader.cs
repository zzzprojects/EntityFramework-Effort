using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_CacheDataLoader : Form
    {
        public Form_Request_CacheDataLoader()
        {
            InitializeComponent();

            //using (var context = new EntityContext(My.ConnectionFactory()))
            //{
            //    context.EntitySimples.Add(new EntitySimple() { ColumnInt = 2});
            //    context.SaveChanges();
            //}
                
            Effort.EntityFrameworkEffortManager.ContextFactory = context => new EntityContext(My.ConnectionFactory());
            var actualDataLoader = new Effort.DataLoaders.EntityDataLoader();
            var cachedDataLoader = new Effort.DataLoaders.CachingDataLoader(actualDataLoader);
            var conn = Effort.DbConnectionFactory.CreateTransient(cachedDataLoader);

            // CLEAN
            using (var context = new EntityContext(conn))
            {
                var list = context.EntitySimples.ToList();
            }

            //// SEED
            //using (var context = new EntityContext(connection))
            //{
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 2 });
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
            //    context.SaveChanges();
            //}

            //// TEST
            //using (var context = new EntityContext(connection))
            //{
            //}
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