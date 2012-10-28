
namespace Effort.Internal.CommandActions
{
    internal class CommandActionParameter
    {
        public CommandActionParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }
}
