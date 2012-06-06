using System.Data.Common;
using System.Data.Entity;

namespace Effort.Test.Data.DbContextSchema
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbConnection connection) : base(connection, true)
        {

        }

        public IDbSet<Person> People { get; set; }
    }
}
