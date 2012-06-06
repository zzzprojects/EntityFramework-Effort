using System;

namespace Effort.Internal.Common
{
    internal class FieldDescription
    {
        public FieldDescription(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }
    }
}
