using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Effort.Provider;

namespace Effort.Shared.Provider
{
    public class EffortRestorePoint
    {
#if !EFOLD
        public EffortConnection EffortConnection { get; set; }
        public List<EffortRestorePointEntry> Entities { get; set; } = new List<EffortRestorePointEntry>();
        private List<EffortRestorePointEntry> OrderedEntities { get; set; }

        public EffortRestorePoint(EffortConnection effortConnection)
        {
            EffortConnection = effortConnection;
        }

        public void AddToIndex(object table, List<object> entities)
        {
            foreach (var entity in entities)
            {
                var itemDeserialized = ShallowCopy(entity);
                Entities.Add(new EffortRestorePointEntry(table, itemDeserialized));
            }
        }

        public void Restore(DbContext context)
        {
            if (OrderedEntities == null)
            {
                CreateOrderedEntities();
                EffortConnection.ClearTables(context);
            }

            foreach (var entity in OrderedEntities)
            {
                var table = entity.Table;
                var methods = table.GetType().GetMethods().Where(x => x.Name == "Insert").ToList()[0];
                var obj = ShallowCopy(entity.Entity);
                methods.Invoke(table, new[] {obj});
            }
        }

        public void CreateOrderedEntities()
        {
            var orderedEntities = new List<EffortRestorePointEntry>();
            var listToTryInsert = new List<EffortRestorePointEntry>();

            // Initialize list to insert
            foreach (var entity in Entities)
            {
                listToTryInsert.Add(new EffortRestorePointEntry(entity.Table, entity.Entity));
            }

            Exception lastError = null;

            while (listToTryInsert.Count > 0)
            {
                var remainingList = new List<EffortRestorePointEntry>();

                foreach (var itemToTry in listToTryInsert)
                    try
                    {
                        var method = itemToTry.Table.GetType().GetMethods().Where(x => x.Name == "Insert").ToList()[0];
                        var obj = ShallowCopy(itemToTry.Entity);

                        method.Invoke(itemToTry.Table, new[] {obj});
                        orderedEntities.Add(itemToTry);
                    }
                    catch (Exception ex)
                    {
                        lastError = ex;
                        remainingList.Add(itemToTry);
                    }

                if (listToTryInsert.Count == remainingList.Count && lastError != null)
                {
                    throw new Exception("Oops! There is an error when trying to generate the insert order.", lastError);
                }

                listToTryInsert = remainingList;
            }

            OrderedEntities = orderedEntities;
        }

        public static T ShallowCopy<T>(T @this)
        {
            var method = @this.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T) method.Invoke(@this, null);
        }

        public class EffortRestorePointEntry
        {
            public EffortRestorePointEntry(object table, object entity)
            {
                Table = table;
                Entity = entity;
            }

            public object Table { get; set; }
            public object Entity { get; set; }
        }
#endif
    }
}
