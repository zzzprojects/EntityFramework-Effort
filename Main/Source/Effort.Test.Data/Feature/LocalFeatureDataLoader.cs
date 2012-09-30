namespace Effort.Test.Data.Feature
{
    using System;
    using System.IO;
    using Effort.DataLoaders;

    public class FeatureLocalDataLoader : CsvDataLoader
    {
        public FeatureLocalDataLoader()
            : base(FindFeatureContent())
        {

        }

        private static string FindFeatureContent()
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                ".\\..\\..\\..\\Effort.Test.Data\\Feature\\Content");
        }
    }
}
