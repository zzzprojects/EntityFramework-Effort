using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Shared.Provider
{
    public class EffortRestorePoint
    {
        public Dictionary<Object, List<Object>> Indexes { get; set; }

        public EffortRestorePoint()
        {
            Indexes = new Dictionary<Object, List<Object>>();
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
                    methods.Invoke(table, new[] { item }); 
                } 
            }
        }
    }
}
