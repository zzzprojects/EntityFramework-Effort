namespace Effort.Test.Data.Initial
{
    using System;
    using System.IO;
    using Effort.DataLoaders;

    public class NorthwindLocalDataLoader : CsvDataLoader
    {
        public NorthwindLocalDataLoader()
            : base(FindNorthwindContent())
        {

        }

        private static string FindNorthwindContent()
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ".\\..\\..\\..\\Effort.Test.Data\\Northwind\\Content");
        }
    }
}
