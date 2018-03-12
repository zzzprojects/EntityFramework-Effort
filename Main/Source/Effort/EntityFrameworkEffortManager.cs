#if !EFOLD
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Effort
{
    /// <summary>Manager for entity framework efforts.</summary>
    public class EntityFrameworkEffortManager
    {
        /// <summary>The context factory.</summary>
        public static Func<DbContext, DbContext> ContextFactory;


        internal static DbContext CreateFactoryContext(DbContext context) 
        {
            if (ContextFactory != null) 
            {
                return ContextFactory(context);
            }

            if (context != null)
            {
                var type = context.GetType();

                var emtptyConstructor = type.GetConstructor(new Type[0]);

                if (emtptyConstructor != null)
                {
                    return (DbContext)emtptyConstructor.Invoke(new object[0]);
                } 
            } 

            throw new Exception("The specified code require a ContextFactory to work. Example: EntityFrmeworkEffortManager.ContextFactory = (currentContext) => new EntitiesContext()");
        }
    }
}
#endif