#if !EFOLD
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NMemory;

namespace Effort
{
    /// <summary>Manager for entity framework efforts.</summary>
    public class EntityFrameworkEffortManager
    {
		// need text
	    public static string PathCustomeManifest = null;

		/// <summary>The context factory.</summary>
		public static Func<DbContext, DbContext> ContextFactory;

        /// <summary>
        /// Gets or sets a value indicating if a default value should be used for a not nullable column
        /// with a null value.
        /// </summary>
        /// <value>
        /// A value indicating if a default value should be used for a not nullable column with a null
        /// value.
        /// </value>
	    public static bool UseDefaultForNotNullable
        {
		    get { return NMemoryManager.UseDefaultForNotNullable; }
		    set { NMemoryManager.UseDefaultForNotNullable = value; }
	    }


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