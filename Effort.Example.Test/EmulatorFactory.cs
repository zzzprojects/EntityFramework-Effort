using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.TddTest.Models;
using MMDB.EntityFrameworkProvider.Tdd;
using System.IO;

namespace MMDB.EntityFrameworkProvider.TddTest.UnitTest
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
