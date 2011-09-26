using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace MMDB.DatabaseExport
{
    public class CultureScope : IDisposable
    {
        private CultureInfo original;

        public CultureScope(CultureInfo cultureInfo)
        {
            this.original = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = this.original;
        }
    }
}
