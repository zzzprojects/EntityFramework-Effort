using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Logging;
using System.Diagnostics;

namespace MMDB.EntityFrameworkProvider.Components
{
    public class Logger : ILoggingPort
    {
        public static bool Enabled { set; get; }

        static Logger()
        {
            Logger.Enabled = true;
        }

        public void Send(MMDBMessage msg)
        {
            if (Logger.Enabled)
            {
                Console.WriteLine(msg);
            }
        }
    }
}
