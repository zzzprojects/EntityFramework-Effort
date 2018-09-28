using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Shared.Provider
{
    public class Snapshot
    {
        public Dictionary<Object, List<Object>> AllIndex { get; set; }

        public Snapshot()
        {
            AllIndex = new Dictionary<Object, List<Object>>();
        }

        public void UseSnapshot()
        {
            foreach (var Entity in AllIndex)
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
