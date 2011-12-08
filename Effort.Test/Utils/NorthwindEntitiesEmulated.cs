using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Test.Data;
using System.Data.EntityClient;
using System.IO;

namespace Effort.Test.Utils
{
    public class NorthwindEntitiesEmulated : NorthwindEntities
    {

        public NorthwindEntitiesEmulated()
            : this("name=NorthwindEntities")
        {
 
        }

        public NorthwindEntitiesEmulated(string connectionString)
            : base(CreateEntityConnection(connectionString))
        {

        }

        private static EntityConnection CreateEntityConnection(string connectionString)
        {
            string csv = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\..\\..\\..\\Effort.Example.Test\\Data");

            return EntityConnectionFactory.CreateEmulator(connectionString, csv, false);
        }

    }
}
