
namespace Effort.Internal.Diagnostics
{
    internal interface ILogger
    {
        void Write(string message);

        void Write(string message, params object[] args);
    }
}
