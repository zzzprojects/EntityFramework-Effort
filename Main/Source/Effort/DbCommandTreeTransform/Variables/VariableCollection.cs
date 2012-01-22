using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Effort.DbCommandTreeTransform.Variables
{
    internal class VariableCollection
    {
        private Dictionary<string, Variable> variables;

        public VariableCollection()
        {
            this.variables = new Dictionary<string,Variable>();
        }

        public void Add(Variable context)
        {
            if (this.variables.ContainsKey(context.Name))
            {
                throw new InvalidOperationException();
            }

            this.variables.Add(context.Name, context);
        }

        public Variable GetVariable(string name)
        {
            Variable context = null;

            if (!this.variables.TryGetValue(name, out context))
            {
                throw new InvalidOperationException();
            }

            return context;
        }


        public void Delete(Variable context)
        {
            if (!variables.Remove(context.Name))
            {
                throw new InvalidOperationException();
            }
        }
    }
}
