using System.Linq;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Feature;

namespace Effort.Test
{
    [TestClass]
    public class TimestampFixture
    {
        private FeatureObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = new LocalFeatureObjectContext();
        }

        [TestMethod]
        public void Feature_TimestampQueryEmulated()
        {
            TimestampSupport timestamp = context.TimestampSupports.FirstOrDefault();

            Assert.IsNotNull(timestamp);
            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void Feature_TimestampInsertEmulated()
        {
            TimestampSupport timestamp = new TimestampSupport();
            timestamp.Description = "New record";

            context.TimestampSupports.AddObject(timestamp);
            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void Feature_TimestampUpdateEmulated()
        {
            TimestampSupport timestamp = context.TimestampSupports.FirstOrDefault();
            byte[] currentValue = timestamp.Timestamp;

            timestamp.Description += "(updated)";
            
            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Select((v, i) => v != currentValue[i]).Any(x => x));
            //Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }
    }
}
