// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestInjector.Injection.NativeInjector;
using TestInjector.TestsTarget;

namespace TestInjector.Tests.Injection
{
   [TestClass]
    public  class InjectorTests
    {
       [TestMethod]
       public void EnsureMethodCanBeInjected()
       {
           string targetAssembly = typeof (App).Assembly.Location;

           Process process = Process.Start(targetAssembly);
           process.WaitForInputIdle();
           try
           {
               Thread.Sleep(1000);

               InjectorMethods.InjectMethod(process.MainWindowHandle,
                                            typeof (InjectorTests),
                                            "InjectedMethod",
                                            "myparam");

               process.WaitForInputIdle();

               Assert.AreEqual("New title: myparam", process.MainWindowTitle);
           }
           finally
           {
               if (process != null)
                   process.Kill();
           }
       }

       public static void InjectedMethod(string param)
       {
           App.Current.MainWindow.Title = "New title: " + param;
       }
    }
}
