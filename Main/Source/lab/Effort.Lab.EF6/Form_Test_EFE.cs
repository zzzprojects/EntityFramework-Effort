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

namespace Effort.Lab.EF6
{
    public partial class Form_Test_EFE : Form
    {
        public Form_Test_EFE()
        {
            InitializeComponent();
            var connection = Effort.DbConnectionFactory.CreateTransient();

            // CLEAN
 
            // SEED
            using (var context = new EntityContext(connection))
            {
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 2 });
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 34 });
                context.BulkSaveChanges();
            }


            using (var context = new EntityContext2(connection))
            {
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 1 });
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 22 });
                context.EntitySimples.Add(new EntitySimple { ColumnInt = 3 });
                context.BulkSaveChanges();
            }


            var connection2 = Effort.DbConnectionFactory.CreateTransient();
            using (var context = new EntityContext3(connection2))
            {
                context.Database.CreateIfNotExists();
                context.TestEFs.Add(new TestEF { ColumnInt = 1, B = Byte.MaxValue , binary = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 } });
                context.TestEFs.Add(new TestEF { ColumnInt = 22 });
                context.TestEFs.Add(new TestEF { ColumnInt = 3 });
                context.BulkSaveChanges();
            }
            // TEST
            using (var context = new EntityContext(connection))
            {
                var list = context.EntitySimples.ToList();
            }
            using (var context = new EntityContext2(connection))
            {
                var list = context.EntitySimples.ToList();
            }
            using (var context = new EntityContext3(connection2))
            {
                var list = context.TestEFs.Where(x => x.B == Byte.MaxValue).ToList();
                list.ForEach(x => x.B = 133);
                context.BulkUpdate(list);
            }

            using (var context = new EntityContext3(connection2))
            {
                var list = context.TestEFs.Where(x => x.B == 133).ToList();
 
            }

            List<TestEF> listTracking = new List<TestEF>();
            using (var context = new EntityContext3(connection2))
            {
                listTracking = context.TestEFs.ToList();
                listTracking.ForEach(x =>
                {
                    x.B = 122;
                    x.binary = new byte[] {0x20, 0x20, 0x20, 0x20, 0x20};
                });

                listTracking.Add(new TestEF() { B = 122 , binary = new byte[] {  0x20, 0x20, 0x20, 0x20 }, ColumnInt =  555});

                context.BulkMerge(listTracking);

                var list = context.TestEFs.ToList();
                list.ForEach(x =>
                {
                    x.B = 123;
                    x.binary = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20 };
                });

                context.TestEFs.Remove(list.ElementAt(0));
                context.TestEFs.Remove(list.ElementAt(1));
                context.BulkSaveChanges();
            } 

            if (listTracking.Where(x => x.B == 123).ToList().Count !=3)
            {
                throw new Exception("ERROER");
            }

            using (var context = new EntityContext3(connection2))
            {
                var list = context.TestEFs.ToList();
                list.ForEach(x =>
                {
                    x.B = 125;
                    x.binary = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20 };
                });

                context.TestEFs.Remove(list.ElementAt(0)); 
                context.BulkSaveChanges(false);
            }
            if (listTracking.Where(x => x.B == 125).ToList().Count != 3)
            {
                throw new Exception("ERROER");
            }

            using (var context = new EntityContext3(connection2))
            {
                var list = context.TestEFs.Where(x => x.B == 123).ToList();
                var testa = context.TestEFs.Where(x => x.binary == new byte[] {0x20, 0x20, 0x20, 0x20, 0x20}).ToList();

                context.BulkDelete(list);
            }


            using (var context = new EntityContext3(connection2))
            {
                var list = context.TestEFs.ToList();
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

        public class EntityContext2 : DbContext
        {
            public EntityContext2(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<EntitySimple> EntitySimples { get; set; }


            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
            }
        }


        public class EntityContext3 : DbContext
        {
            public EntityContext3(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<TestEF> TestEFs { get; set; }


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

        public class TestEF
        {
            public int ID { get; set; }
            public int ColumnInt { get; set; }
            public byte B { get; set; }

            [Column("binary", TypeName = "binary")]
            public byte[] binary { get; set; }
        }
    }
}