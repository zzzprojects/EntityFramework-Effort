// ----------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// ----------------------------------------------------------------------------------

namespace Effort.CsvTool.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ProviderViewModel> providers;
        private ICommand exportCommand;

        private ProviderViewModel selectedProvider;
        private DbConnectionStringBuilder connectionStringBuilder;
        private int reportProgress;

        private BackgroundWorker worker;

        public MainViewModel()
        {
            this.providers = new ObservableCollection<ProviderViewModel>();

            foreach (DataRow item in DbProviderFactories.GetFactoryClasses().Rows)
            {
                this.providers.Add(new ProviderViewModel((string)item["Name"], (string)item["AssemblyQualifiedName"]));
            }

            this.worker = new BackgroundWorker();

            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            this.worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            this.SelectedProvider = this.providers.FirstOrDefault();
            this.exportCommand = new RelayCommand(Export);

            this.ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }


        public ObservableCollection<ProviderViewModel> Providers
        {
            get 
            { 
                return this.providers; 
            }
        }

        public ProviderViewModel SelectedProvider
        {
            get
            {
                return this.selectedProvider;
            }
            set
            {
                this.selectedProvider = value;
                this.SetupProperties();
            }
        }

        public DbConnectionStringBuilder ConnectionStringBuilder 
        { 
            get 
            { 
                return this.connectionStringBuilder; 
            }
            set
            {
                this.connectionStringBuilder = value;
                base.NotifyChanged("ConnectionStringBuilder");
            }
        }

        public string ExportPath { get; set; }

        public int ReportProgress 
        {
            get { return this.reportProgress; }
            set 
            {
                this.reportProgress = value;
                base.NotifyChanged("ReportProgress");
            }
        }


        public ICommand ExportCommand
        {
            get { return this.exportCommand; }
        }


        private void SetupProperties()
        {
            ProviderViewModel provider = this.SelectedProvider;

            if (provider == null)
            {
                return;
            }

            var factory = provider.GetProviderFactory();
            var connectionStringBuilder = factory.CreateConnectionStringBuilder();

            this.ConnectionStringBuilder = connectionStringBuilder;
        }

        private void Export(object arg)
        {
            ProviderViewModel provider = this.SelectedProvider;

            if (provider == null)
            {
                return;
            }

            var factory = provider.GetProviderFactory();

            var connection = factory.CreateConnection();

            connection.ConnectionString = this.ConnectionStringBuilder.ConnectionString;

            WorkerArgs args = new WorkerArgs()
            {
                Connection = connection,
                ExportPath = this.ExportPath
            };

            this.worker.RunWorkerAsync(args);
        }


        private class WorkerArgs
        {
            public DbConnection Connection { get; set; }
            public string ExportPath { get; set; }
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerArgs args = e.Argument as WorkerArgs;
            var con = args.Connection;
            var dir = new DirectoryInfo(args.ExportPath);

            using(new CultureScope(CultureInfo.InvariantCulture))
            using (con)
            {
                con.Open();

                var schema = con.GetSchema("Tables");

                List<string> tables = new List<string>();

                foreach (DataRow item in schema.Rows)
                {
                    if (item[3].Equals("BASE TABLE"))
                    {
                        string name = item[2] as string;

                        tables.Add(name);
                    }
                }

                for (int j = 0; j < tables.Count; j++ )
                {
                    string name = tables[j];

                    using (DbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = string.Format("SELECT * FROM [{0}]", name);
                        cmd.CommandType = CommandType.Text;

                        FileInfo file = new FileInfo(Path.Combine(dir.FullName, string.Format("{0}.csv", name)));

                        if (!dir.Exists)
                        {
                            dir.Create();
                        }

                        using (DbDataReader reader = cmd.ExecuteReader())
                        using (StreamWriter sw = new StreamWriter(file.Open(FileMode.Create, FileAccess.Write, FileShare.None)))
                        {
                            int fieldCount = reader.FieldCount;

                            string[] fieldNames = new string[fieldCount];
                            Func<object, string>[] serializers = new Func<object, string>[fieldCount];

                            for (int i = 0; i < fieldCount; i++)
                            {
                                fieldNames[i] = reader.GetName(i);

                                Type fieldType = reader.GetFieldType(i);

                                if (fieldType == typeof(Byte[]))
                                {
                                    serializers[i] = BinarySerializer;
                                }
                                else if (fieldType == typeof(string))
                                {
                                    serializers[i] = StringSerializer;
                                }
                                else
                                {
                                    // Default serializer
                                    serializers[i] = DefaultSerializer;
                                }

                            }

                            sw.WriteLine(string.Join(",", fieldNames));

                            object[] values = new object[fieldCount];
                            string[] serializedValues = new string[fieldCount];

                            while (reader.Read())
                            {
                                reader.GetValues(values);

                                for (int i = 0; i < fieldCount; i++)
                                {
                                    object value = values[i];

                                    // Check if null
                                    if (value == null || value is DBNull)
                                    {
                                        serializedValues[i] = "";
                                    }
                                    else
                                    {
                                        serializedValues[i] = serializers[i](value);
                                    }
                                }

                                for (int i = 0; i < fieldCount - 1; i++)
                                {
                                    sw.Write("\"{0}\"", ConvertToCsv(serializedValues[i]));
                                    sw.Write(',');
                                }

                                sw.WriteLine("\"{0}\"", ConvertToCsv(serializedValues[fieldCount - 1]));

                            }
                            // DataReader is finished
                        }
                        // Command is finished
                    }

                    this.worker.ReportProgress((int)((double)(j + 1) * 100.0 / tables.Count));

                    // Table is finished
                }
                // All table finished
            }
            // Connection is closed
        }

        private static string ConvertToCsv(string input)
        {
            return input
                .Replace("\"", "\"\"")
                .Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
        }

        private static string DefaultSerializer(object input)
        {
            return input.ToString();
        }

        private static string StringSerializer(object input)
        {
            return "'" + input.ToString();
        }

        private static string BinarySerializer(object input)
        {
            byte[] bin = input as byte[];

            return Convert.ToBase64String(bin);
        }


        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ReportProgress = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.ReportProgress = 0;
        }
    }




}
