// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestInjector.Common;
using TestInjector.Service;

namespace TestInjector.Client
{
    public class TestRunner
    {
        public async void StartDoingStuffAsync()
        {
            TestLog.Current = new TestLog();

            ResolveEventHandler resolveAction = null;

            var testDetails = await GetTestDetailsAsync();

            List<Assembly> newAssemblies = new List<Assembly>();
            resolveAction = await LoadAssembliesAsync(testDetails, newAssemblies);

            var method = await FindMethodToRunAsync(testDetails, newAssemblies);

            if (method != null)
            {
                using (CaptureTrace())
                {
                    Exception exceptionToReport = null;
                    try
                    {

                        var result = method.Invoke(null, null);
                        if (result is Task)
                        {
                            await ((Task) result);
                        }

                        await ReportTestProgressAsync(TestStatus.Success);
                    }
                    catch (Exception ex)
                    {
                        exceptionToReport = ex;
                    }

                    if (exceptionToReport != null)
                    {
                        await ReportTestProgressAsync(
                            TestStatus.Exception,
                            new ExceptionDetails()
                                {
                                    ExceptionString = exceptionToReport.ToString(),
                                    Message = exceptionToReport.Message,
                                    ExceptionType = exceptionToReport.GetType().FullName
                                });
                    }
                }

            }
            else
            {
                await
                    ReportTestProgressAsync(TestStatus.Failed, 
                    new ExceptionDetails() { Message = "Couldn't find method: " + testDetails.TypeName + "." + testDetails.MethodName });
            }

            if (resolveAction != null)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolveAction;
            }
        }

        private async Task ReportTestProgressAsync(TestStatus status, ExceptionDetails exceptionDetails = null)
        {
            await Task.Run(() =>
                                 {
                                     using (var client = TestCallbackClient.Create())
                                     {
                                         client.ReportTestProgress("ThisTest",
                                                                   status,
                                                                   exceptionDetails,
                                                                   TestLog.Current.MessageDetails);
                                     }
                                 });
        }

        private async Task<TestDetails> GetTestDetailsAsync()
        {
            // Debugger.Launch();
            return await Task.Run(
                () =>
                {
                    using (var client = TestCallbackClient.Create())
                    {
                        return client.GetMethodToRun();
                    }
                });

        }

        private Task<ResolveEventHandler> LoadAssembliesAsync(TestDetails details, List<Assembly> newAssemblies)
        {
            return Task.Run(
                () =>
                {
                    string thisAssembly = GetType().Assembly.FullName;
                    string thisAssemblyLocation = GetType().Assembly.Location;
                    ResolveEventHandler resolveAction = (o, eventArgs) =>
                    {
                        // If the assembly name matches, then use the dll....

                        if (eventArgs.Name == thisAssembly)
                        {
                            return Assembly.LoadFrom(thisAssemblyLocation);
                        }

                        var matchingRef = details.AssembliesToLoad
                            .FirstOrDefault(
                                x => x.AssemblyName == eventArgs.Name);

                        if (matchingRef != null)
                        {
                            if (
                                string.IsNullOrWhiteSpace(
                                    matchingRef.FileName))
                            {
                                return
                                    Assembly.Load(matchingRef.AssemblyName);
                            }
                            else
                            {
                                return
                                    Assembly.LoadFrom(matchingRef.FileName);
                            }
                        }
                        return null;
                    };

                    AppDomain.CurrentDomain.AssemblyResolve += resolveAction;

                    // Load current assembly
                    Assembly.LoadFrom(GetType().Assembly.Location);


                    if (details.AssembliesToLoad != null)
                    {
                        foreach (var assemblyName in details.AssembliesToLoad)
                        {
                            if (string.IsNullOrWhiteSpace(assemblyName.FileName))
                            {
                                Assembly.Load(assemblyName.AssemblyName);
                            }
                            else
                            {
                                var assembly = Assembly.LoadFrom(assemblyName.FileName);

                                newAssemblies.Add(assembly);
                            }
                        }
                    }

                    return resolveAction;
                });
        }

        private async Task<MethodInfo> FindMethodToRunAsync(TestDetails details, List<Assembly> newAssemblies)
        {
            return await Task.Run(() =>
            {
                Type methodType = null;
                foreach (var assembly in newAssemblies)
                {
                    methodType = assembly.GetType(details.TypeName);
                    if (methodType != null)
                        break;
                }

                if (methodType != null)
                {
                    // var methodType = Type.GetType(details.TypeName);
                    return methodType.GetMethod(details.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                }
                return null;
            });
        }

        public class LocalTraceListener : TraceListener
        {
            public override void Write(string message)
            {
                TestLog.Current.Trace(message);
            }

            public override void WriteLine(string message)
            {
                TestLog.Current.Trace(message);
            }
        }

        private IDisposable CaptureTrace()
        {
            var listener = new LocalTraceListener();

            // add a listener to the standard trace
            Trace.Listeners.Add(listener);

            return new DisposableAction(() =>
                                            {
                                                Trace.Listeners.Remove(listener);
                                                listener.Dispose();
                                            });
        }

       
    }
}
