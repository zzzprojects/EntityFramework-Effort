using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Effort.Example.Models;

namespace Effort.Example.Test.UnitTest
{
    public static class EmulatorFactory
    {
        private static Type emulator;

        public static Type Create()
        {
            if (emulator == null)
            {
                string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

                emulator = ObjectContextFactory.CreateEmulator<NorthwindEntities>(baseDir, false);
            }

            return emulator;
        }
    }
}
