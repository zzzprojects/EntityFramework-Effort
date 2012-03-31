using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Diagnostics
{
    internal interface ILogger
    {
        void Write(string message);

        void Write(string message, params object[] args);
    }
}
