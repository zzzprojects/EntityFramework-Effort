using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Effort.Shared.Provider
{
    public class EffortRestorePoint
    {
        public Dictionary<object, List<object>> Indexes { get; set; }

        public EffortRestorePoint()
        {
            Indexes = new Dictionary<object, List<object>>();
        }

        public void AddToIndex(object obj, List<object> entities)
        {
            var list = new List<object>();

            foreach (var entity in entities)
            {
                var itemDeserialized = ShallowCopy(entity);
                list.Add(itemDeserialized);
            }

            Indexes.Add(obj, list);
        }


        public void Restore()
        {
            foreach (var Entity in Indexes)
            {
                var table = Entity.Key;
                var dynamicTable = (dynamic)table;
                var methods = table.GetType().GetMethods().Where(x => x.Name == "Insert").ToList()[0];

                foreach (var item in Entity.Value)
                {
                    var obj = ShallowCopy(item);
                    methods.Invoke(table, new[] { obj }); 
                } 
            }
        }

        public static T ShallowCopy<T>(T @this)
        {
            MethodInfo method = @this.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(@this, null);
        }
    }
}
