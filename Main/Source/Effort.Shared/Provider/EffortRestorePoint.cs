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
        public Dictionary<object, List<object>> Entities { get; set; }

        public EffortRestorePoint()
        {
            Entities = new Dictionary<object, List<object>>();
        }

        public void AddToIndex(object obj, List<object> entities)
        {
            var list = new List<object>();

            foreach (var entity in entities)
            {
                var itemDeserialized = ShallowCopy(entity);
                list.Add(itemDeserialized);
            }

            Entities.Add(obj, list);
        }


        public void Restore()
        {
            foreach (var entity in Entities)
            {
                var table = entity.Key;
                var methods = table.GetType().GetMethods().Where(x => x.Name == "Insert").ToList()[0];

                foreach (var item in entity.Value)
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
