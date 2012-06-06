using System.Linq;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class TimestampFixture
    {
        private FeatureEntities emulatedContext;

        [TestInitialize]
        public void Initialize()
        {
            this.emulatedContext = new FeatureEntitiesEmulated();
        }

        [TestMethod]
        public void Feature_TimestampQueryEmulated()
        {
            TimestampSupport timestamp = emulatedContext.TimestampSupports.FirstOrDefault();

            Assert.IsNotNull(timestamp);
            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void Feature_TimestampInsertEmulated()
        {
            TimestampSupport timestamp = new TimestampSupport();
            timestamp.Description = "New record";

            emulatedContext.TimestampSupports.AddObject(timestamp);
            emulatedContext.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void Feature_TimestampUpdateEmulated()
        {
            TimestampSupport timestamp = emulatedContext.TimestampSupports.FirstOrDefault();
            byte[] currentValue = timestamp.Timestamp;

            timestamp.Description += "(updated)";
            
            emulatedContext.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Select((v, i) => v != currentValue[i]).Any(x => x));
            //Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }
    }
}
