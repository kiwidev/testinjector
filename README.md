Test Injector
=======

TestInjector is a NuGet package allowing you to inject Unit Tests into your running WPF (or soon to come WinForms) application.

This means that instead of having to do all work via UI Automation, unit tests on a running application can be performed by calling ViewModels and UI views directly.

### Getting Started

 - Open your project in Visual Studio
 - `Install-Package TestInjector -Pre` from the Package Manager Console
 - Ensure your unit tests are running using the same architecture (x86/x64) as your application
 - Reference your target application from your unit tests (this can be the same project as conventional unit tests).

### Using it

        [TestMethod]
        public void TestThatSomethingInsideMyApplicationWorks()
        {
            using (var applicationInstance = new ApplicationInstance())
            {
                applicationInstance.Launch("MyApp.exe");

                applicationInstance.Run(async () =>
                    {
                        // This code runs on the UI thread of the application
                        // yes, async is available
                        await Task.Delay(5000);

                        // Using methods defined in the target application
                        await IoC.Get<IEventAggregator>().Publish(new OpenWindowEvent());


                        // Or using UI code
                        var myWindow = Application.Current.MainWindow;
                        myWindow.TextBox1.Text = "Bob";
                    });
            }
        }

## About
![Datacom](https://raw.github.com/wiki/kiwidev/testinjector/images/datacom.jpg)

This project is supported by [Datacom](http://www.datacom.co.nz)

### License
Licensed under MS-PL, please see [License.md](LICENSE.md) for details.


