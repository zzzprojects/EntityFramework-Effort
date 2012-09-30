using System.Data.Common;
using System.Data.Entity;

namespace Effort.Test.Data.Staff
{
    public class StaffDbContext : DbContext
    {
        public StaffDbContext(DbConnection connection) : base(connection, true)
        {

        }

        public IDbSet<Person> People { get; set; }
    }
}
