// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestInjector;
using TestInjectorSample.WpfApplication45.Tests.Properties;

namespace TestInjectorSample.WpfApplication45.Tests
{
    [TestClass]
    public class LambdaTest
    {
        [TestMethod]
        public void UpdateUiControls()
        {
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch(Settings.Default.TestApplicationLocation);

                applicationInstance.RunMethod(() => UpdateUiControlsImpl());
            }
        }

        private static void UpdateUiControlsImpl()
        {
            
        }

        [TestMethod]
        public void AnonymousMethodsAreAbleToBeUsed()
        {
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch(Settings.Default.TestApplicationLocation);
                
                applicationInstance.Run(() =>
                    {
                        Thread.Sleep(5000);
                    });
            }
        }

        [TestMethod]
        public void AnonymousMethodsWithAsyncAreAbleToBeUsed()
        {
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch(Settings.Default.TestApplicationLocation);

                applicationInstance.Run(async () =>
                    {
                        await Task.Delay(5000);
                    });
            }
        }


    }
}
