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

namespace Effort.Lab.EF6
{
    public partial class Form_Issue_DbComparisonExpressionTime : Form
    {
        public Form_Issue_DbComparisonExpressionTime()
        {
            InitializeComponent();
            var connection = Effort.DbConnectionFactory.CreateTransient();

            //// CLEAN
            //using (var context = new EntityContext(connection))
            //{
            //    context.EntitySimples.RemoveRange(context.EntitySimples);
            //    context.SaveChanges();
            //}

            //// SEED
            //using (var context = new EntityContext(connection))
            //{
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 1, StarDateTime = new DateTime(2010,01,01), EndDateTime = new DateTime(2020, 01, 01) });
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 2, StarDateTime = new DateTime(2010, 01, 01) } );
            //    context.EntitySimples.Add(new EntitySimple { ColumnInt = 3, StarDateTime = new DateTime(2010, 01, 01), EndDateTime = new DateTime(2020, 01, 01) } );
            //    context.SaveChanges();
            //}

            // TEST
            using (var context = new EntityContext(connection))
            {
                //var anyResult1 = context.EntitySimples
                //    .AsQueryable()
                //    .Select(t => new { t.StarDateTime, EndDate = t.EndDateTime ?? new DateTime(9999, 12, 31) })
                //    .Any(t => t.StarDateTime >= t.EndDate);

                DateTime maxDate = new DateTime(9999, 12, 31);

                var anyResult2 = context.EntitySimples
                    .AsQueryable()
                    .Select(t => new { t.StarDateTime, EndDate = t.EndDateTime ?? maxDate })
                    .Any(t => t.StarDateTime >= t.EndDate);
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public EntityContext() : base("CodeFirstEntities")
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
            public DateTime StarDateTime { get; set; }
            public DateTime? EndDateTime { get; set; }
        }
    }
}