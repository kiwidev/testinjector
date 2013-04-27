// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestInjector;
using TestInjectorSample.WpfApplication45.Tests.Properties;

namespace TestInjectorSample.WpfApplication45.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void ApplicationCanBeLaunchedAndAttachedTo()
        {
            Process process;
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch(Settings.Default.TestApplicationLocation);
                process = applicationInstance.Process;
                Assert.IsNotNull(applicationInstance.Process);
            }

            Assert.IsTrue(process.HasExited);
        }

        [TestMethod]
        public void UpdateUiControls()
        {
            using (var applicationInstance = new ApplicationInstance(Settings.Default.TestApplicationLocation))
            {
                applicationInstance.Run(async () =>
                    {
                        // This is running on the UI thread
                        // Debugger.Launch();

                        var window = (Application.Current.MainWindow as MainWindow);
                        if (window == null)
                            Assert.Fail("No main window");

                        Assert.AreEqual("Initial heading", window.HeadingLabel.Text);

                        window.HeadingTextBox.Text = "Our text";

                        ClickButton(window.Go);

                        // Give it time to do its thing
                        await Task.Delay(500);

                        Assert.AreEqual("Our text", window.HeadingLabel.Text);
                    });
            }
        }

        /// <summary>
        /// Helper method to programmatic click a WPF button
        /// </summary>
        /// <param name="button"></param>
        private static void ClickButton(Button button)
        {
            ButtonAutomationPeer peer =
                new ButtonAutomationPeer(button);
            IInvokeProvider invokeProv =
                peer.GetPattern(PatternInterface.Invoke)
                as IInvokeProvider;
            invokeProv.Invoke();
        }
    }
}
