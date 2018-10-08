using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace Effort.Lab.EF6
{
    public class Request_OrderBy
    {
        public static void Execute()
        {
            var connection = DbConnectionFactory.CreateTransient();

            using (var context = new EntitiesContext(connection))
            {
                for (var i = 0; i < 10; i++)
                {
                    var test = new MyEntity {ColumnText = (i % 3).ToString(), ColumnChar = i.ToString()};

                    context.MyEntities.Add(test);
                }

                context.SaveChanges();
            }

            using (var context = new EntitiesContext(connection))
            {
                var list = context.MyEntities.OrderByDescending(x => x.ColumnText).ThenByDescending(y => y.ColumnChar).ToList();
            }
        }
    }

    public class EntitiesContext : DbContext
    {
        public EntitiesContext(DbConnection dbConnection) : base(dbConnection, true)
        {
        }

        public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string ColumnText { get; set; }

        public string ColumnChar { get; set; }
    }
}