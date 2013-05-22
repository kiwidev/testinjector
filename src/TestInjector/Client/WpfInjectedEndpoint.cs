// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace TestInjector.Client
{
    public static class WpfInjectedEndpoint
    {
        public static void RemoteInjectionPoint()
        {
            StartClientTests();
        }

        public static void StartClientTests()
        {
            Dispatcher dispatcher;
            if (Application.Current == null)
                dispatcher = Dispatcher.CurrentDispatcher;
            else
                dispatcher = Application.Current.Dispatcher;

            if (dispatcher.CheckAccess())
            {
                new TestRunner().StartDoingStuffAsync();
            }
            else
            {
                dispatcher.Invoke((Action)StartClientTests);
            }
        }
    }
}
