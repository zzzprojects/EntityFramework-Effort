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
    public partial class Form_General_Jon : Form
    {
        public Form_General_Jon()
        {
            InitializeComponent();
            {
                var connection = Effort.DbConnectionFactory.CreateTransient();

                // SEED
                using (var context = new EntityContext(connection))
                {

                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 2 });
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
                    context.SaveChanges();

                    connection.Open();
                    connection.DbManager.SetIdentity<EntitySimple>(50, 5);
                    //connection.DbManager.SetIdentity("TableTest", 50, 5);

                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                    context.SaveChanges();
                }

                // TEST
                using (var context = new EntityContext(connection))
                {
                    var list1 = context.EntitySimples.ToList(); 
                }
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

        //[Table("TableTest")]
        public class EntitySimple
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
        }
    }
}