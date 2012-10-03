using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class TestInitialization
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Effort.Provider.EffortProviderConfiguration.RegisterProvider();
            Effort.Test.Tools.DataReaderInspector.DataReaderInspectorProviderConfiguration.RegisterProvider();
        }
    }
}
