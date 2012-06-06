
namespace Effort.Internal.CommandActions
{
    internal class Parameter
    {
        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }
}
