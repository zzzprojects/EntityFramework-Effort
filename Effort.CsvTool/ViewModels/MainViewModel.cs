using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using WPG.Data;
using System.IO;
using System.Globalization;

namespace MMDB.DatabaseExport.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ProviderViewModel> providers;
        private ObservableCollection<Item> properties;
        private ICommand exportCommand;

        private ProviderViewModel selectedProvider;
        private DbConnectionStringBuilder connectionStringBuilder;
        private int reportProgress;

        private BackgroundWorker worker;

        public MainViewModel()
        {
            this.providers = new ObservableCollection<ProviderViewModel>();
            this.properties = new ObservableCollection<Item>();

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

        public ObservableCollection<Item> Properties
        {
            get
            {
                return this.properties;
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
                                    serializedValues[i] = serializers[i](values[i]);
                                }

                                for (int i = 0; i < fieldCount - 1; i++)
                                {
                                    sw.Write("\"{0}\"", serializedValues[i].Replace("\"", "\"\""));
                                    sw.Write(',');
                                }

                                sw.WriteLine("\"{0}\"", serializedValues[fieldCount - 1].Replace("\"", "\"\""));

                            }

                            // DataReader is finished
                        }
                        
                        // Command is finished
                    }

                    this.worker.ReportProgress((int)((double)j * 100.0 / tables.Count));

                    // Table is finished
                }

                // All table finished
            }

            // Connection is closed
        }

        private static string DefaultSerializer(object input)
        {
            return input.ToString();
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
