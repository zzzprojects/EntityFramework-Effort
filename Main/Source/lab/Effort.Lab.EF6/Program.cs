using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Form_Request_TransactionIssue();
        }
        //[STAThread]
        //static async Task Main(string[] args)
        //{
        //    // CODE FIRST - works using persistent connection but won't using transient (throws NullReferenceException in that case)
        //    using (var conn = DbConnectionFactory.CreatePersistent(Guid.NewGuid().ToString()))
        //    {
        //        // these two steps are crucial, otherwise exceptions are thrown
        //        using (var ctx = new CodeFirstDataContext(conn))
        //            ctx.Database.CreateIfNotExists();

        //        conn.Open();

        //        await DoTest(() => new CodeFirstDataContext(conn));
        //    }

        //    // MODEL FIRST - won't work using either persistent or transient (throws EntityException - The underlying provider failed on DataSource)
        //    using (var conn = EntityConnectionFactory.CreateTransient("metadata=res://*/DataContext.csdl|res://*/DataContext.ssdl|res://*/DataContext.msl"))
        //    {
        //        conn.Open();

        //        await DoTest(() => new DataContextContainer(conn));
        //    }
        //}

        //private static async Task DoTest(Func<IDataContext> contextFactory)
        //{
        //    const int n = 60;

        //    await Task.WhenAll(Enumerable.Range(0, n)
        //        .Select(i => Task.Run(() =>
        //        {
        //            IDataContext context = contextFactory();
        //            try
        //            {
        //                var random = new Random(i);
        //                switch (i % 3)
        //                {
        //                    case 0:
        //                        context.Items.Add(new Item { Value = i.ToString() });
        //                        break;
        //                    default:
        //                        var items = context.Items.ToArray();
        //                        var item = items[random.Next(items.Length)];
        //                        item.Value = item.Value + " Modified";
        //                        break;
        //                }
        //                context.SaveChanges();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex);
        //            }
        //            finally
        //            {
        //                context.Dispose();
        //            }
        //        })));

        //    using (var ctx = contextFactory())
        //        Console.WriteLine($"Expected = {n / 3}, Actual = { ctx.Items.Count() }");
        //        }
    }
}
