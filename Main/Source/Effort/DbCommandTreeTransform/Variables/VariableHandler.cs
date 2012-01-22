using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DbCommandTreeTransform.Variables
{
    internal class VariableHandler : IDisposable
    {
        private VariableCollection collection;
        private Variable variable;

        public VariableHandler(Variable variable, VariableCollection collection)
        {
            this.variable = variable;
            this.collection = collection;

            this.collection.Add(variable);
        }

        public Variable Context
        {
            get { return this.variable; }
        }

        public void Dispose()
        {
            this.collection.Delete(variable);
        }
    }
}
