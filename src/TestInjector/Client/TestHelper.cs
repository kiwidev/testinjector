// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;

namespace TestInjector.Client
{
    public static class TestHelper
    {
        /// <summary>
        /// Runs something on the UI Thread.
        /// Note that this blocks
        /// </summary>
        /// <param name="action"></param>
        public static void UIThread(Action action, bool waitForIdleAfterwards = false)
        {
            var dispatcher = Application.Current.Dispatcher;

            dispatcher.Invoke(action, DispatcherPriority.Normal);

            if (waitForIdleAfterwards)
                WaitForIdle();
        }

        public static void WaitForIdle(bool justIdle = false)
        {
            if (!justIdle)
            {
                var process = Process.GetCurrentProcess();


                process.WaitForInputIdle();

                Thread.Sleep(500);

                process.WaitForInputIdle();
            }

            var dispatcher = Application.Current.Dispatcher;

            Action doNotMuch = () => { };
            dispatcher.Invoke(doNotMuch, DispatcherPriority.ContextIdle);

        }


        public static Task WaitForIdleAsync(bool justIdle = false)
        {
            return Task.Run(() =>
            {
                if (!justIdle)
                {
                    var process = Process.GetCurrentProcess();


                    process.WaitForInputIdle();

                    Thread.Sleep(500);

                    process.WaitForInputIdle();
                }


                var dispatcher = Application.Current.Dispatcher;

                Action doNotMuch = () => { };
                dispatcher.Invoke(doNotMuch, DispatcherPriority.ContextIdle);
            });
        }


        public static void WaitFor(Func<bool> func)
        {
            bool res = false;
            while (!res)
            {
                UIThread(() => res = func());

                if (!res)
                    Thread.Sleep(20);
            }
        }



        public static async Task WaitForAsync(Func<bool> func)
        {
            bool res = false;
            while (!res)
            {
                res = func();

                if (!res)
                    await Task.Delay(20);
            }
        }
    }
}
