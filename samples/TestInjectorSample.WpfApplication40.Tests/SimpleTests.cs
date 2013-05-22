using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestInjector;

namespace TestInjectorSample.WpfApplication40.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        [Ignore]
        public void ApplicationCanBeLaunchedAndAttachedTo()
        {
            Process process;
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch(@"PATHTOAPPLICATION");

                process = applicationInstance.Process;
                Assert.IsNotNull(applicationInstance.Process);

                applicationInstance.Run(() => { });
            }

            Assert.IsTrue(process.HasExited);
        }
    }
}