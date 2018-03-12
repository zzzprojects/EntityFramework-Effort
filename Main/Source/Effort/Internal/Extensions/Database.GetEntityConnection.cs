#if !EFOLD

using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Reflection;

namespace Effort.Internal
{
    internal static class InternalExtensions
    {
        internal static EntityConnection GetEntityConnection(this Database database)
        {
            var internalContext = database.GetType().GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(database);

            var getObjectContext = internalContext.GetType().GetMethod("GetObjectContextWithoutDatabaseInitialization", BindingFlags.Public | BindingFlags.Instance);

            var objectContext = (ObjectContext) getObjectContext.Invoke(internalContext, null);

            var entityConnection = objectContext.Connection;

            return (EntityConnection) entityConnection;
        }
    }
}

#endif