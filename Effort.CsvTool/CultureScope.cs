using System;
using System.Globalization;
using System.Threading;

namespace Effort.CsvTool
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
