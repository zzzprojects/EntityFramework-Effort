using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effort.Lab.EF6
{
    public static class My
    {
        public static Func<SqlConnection> ConnectionFactory = () => new SqlConnection("Server=localhost;Initial Catalog=Z.Test.EntityFramework.Plus.EF6;Integrated Security=true;Connection Timeout=300;Persist Security Info=True");
    }
}
