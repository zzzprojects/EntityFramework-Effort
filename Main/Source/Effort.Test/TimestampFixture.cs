using System.Linq;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Objects;

namespace Effort.Test
{
    [TestClass]
    public class TimestampFixture
    {
        private FeatureEntities emulatedContext;
        private FeatureEntities acceleratedContext;

        [TestInitialize]
        public void Initialize()
        {
            this.emulatedContext = new FeatureEntitiesEmulated();
            this.acceleratedContext = new FeatureEntities("name=FeatureEntities");
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

        [TestMethod]
        public void Feature_TimestampQueryAccelerated()
        {
            var timestamp = acceleratedContext.TimestampSupports.FirstOrDefault();

            Assert.IsNotNull(timestamp);
            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }


        [TestMethod]
        public void Feature_TimestampInsertAccelerated()
        {
            TimestampSupport timestamp = new TimestampSupport();
            timestamp.Description = "New record";

            acceleratedContext.TimestampSupports.AddObject(timestamp);
            acceleratedContext.SaveChanges();

            try
            {
                Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
            }
            finally
            {
                // Cleanup
                acceleratedContext.TimestampSupports.DeleteObject(timestamp);
                acceleratedContext.SaveChanges();
            }
        }

        [TestMethod]
        public void Feature_TimestampUpdateAccelerated()
        {
            TimestampSupport timestamp = acceleratedContext.TimestampSupports.FirstOrDefault();
            byte[] currentValue = timestamp.Timestamp;

            string original = timestamp.Description;

            timestamp.Description += "(updated)";

            acceleratedContext.SaveChanges();

            try
            {
                Assert.IsTrue(timestamp.Timestamp.Select((v, i) => v != currentValue[i]).Any(x => x));
            }
            finally
            {
                // Cleanup
                timestamp.Description = original;
                acceleratedContext.SaveChanges();
            }
        }

    }
}
