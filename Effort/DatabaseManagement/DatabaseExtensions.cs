using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Table;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public static class DatabaseExtensions
    {
        public static IReflectionTable GetTable(this Database database, string name)
        {
            return database
                .Tables
                .Cast<IReflectionTable>()
                .Where(t => t.EntityType.Name.Equals(name))
                .First();
        }
    }
}
