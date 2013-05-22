// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using TestInjector.Injection.NativeInjector;
using TestInjector.Service;

namespace TestInjector.Client
{
    public class TestInjectorClient
    {
        public void RunMethod(Process process, Type type, string methodName, TestInjectorSettings testInjectorSettings)
        {
            Trace.WriteLine("Running method: " + type.Name + "." + methodName);

            ExceptionDetails exceptionDetails = null;
            TestStatus? serviceStatus = null;
            string traceLog = null;

            AutoResetEvent resetEvent = new AutoResetEvent(false);
            AutoResetEvent testDetailsRetrievedResetEvent = new AutoResetEvent(false);



            var assembliesToLoad = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => new AssemblyRef
                {
                    AssemblyName = x.FullName,
                    FileName = x.Location
                })
                .ToArray();

            foreach (var assembly in assembliesToLoad)
            {
                if (assembly.FileName.IndexOf(@"Microsoft.NET\Framework", StringComparison.CurrentCultureIgnoreCase) >= 0
                         ||
                        assembly.FileName.IndexOf(@"Microsoft.NET\assembly", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    assembly.FileName = null;
                }
            }

            var processFile = process.MainModule.FileName;
            var processFileDirectory = System.IO.Path.GetDirectoryName(processFile);

            var assembliesToExclude = assembliesToLoad
                .Where(x => x.FileName != null)
                .Where(x => File.Exists(
                    Path.Combine(processFileDirectory, Path.GetFileName(x.FileName))))
                    .ToList();

            assembliesToLoad = assembliesToLoad.Except(assembliesToExclude).ToArray();

            var testDetails = new TestDetails
            {
                AssembliesToLoad = assembliesToLoad,
                MethodName = methodName,
                TypeName = type.FullName
            };

            bool testDetailsRetrieved = false;
            ServiceController.OnTestDetailsRetrieved = () =>
                {
                    testDetailsRetrieved = true;
                    ThreadPool.QueueUserWorkItem(state =>
                        {
                            Thread.Sleep(10);
                            testDetailsRetrievedResetEvent.Set();
                        });
                };

            using (var serviceHost = ServiceController
                .StartService(testDetails, ((name, status, localExceptionDetails, localTraceLog) =>
                                                {
                                                    serviceStatus = status;
                                                    exceptionDetails = localExceptionDetails;
                                                    traceLog = localTraceLog;

                                                    resetEvent.Set();
                                                })))
            {
                try
                {
                    process.Refresh();
                    InjectorMethods.InjectMethod(process.MainWindowHandle,
                                                 typeof (WpfInjectedEndpoint),
                                                 "RemoteInjectionPoint",
                                                 "");
                }
                catch (Exception ex)
                {
                    throw new TestInjectorClientException("Failed to inject process", ex);
                }

                // Timeout out waiting for method to be injected
                if (!testDetailsRetrieved)
                {
                    testDetailsRetrievedResetEvent.WaitOne(TimeSpan.FromSeconds(10));
                }
                if (!testDetailsRetrieved)
                {
                    throw new TestInjectorClientException("Timed out waiting for client injection");
                }

                // TODO: Need a timeout / heartbeat here
                while (serviceStatus == null || serviceStatus.Value == TestStatus.Started)
                {
                    resetEvent.WaitOne(TimeSpan.FromSeconds(10));
                }

                if (serviceStatus == null)
                {
                    throw new TestInjectorClientException("Timed out");
                }

                if (traceLog != null)
                {
                    Trace.WriteLine("Method details:\r\n" + traceLog);
                }

                // Failed to call the method
                if (serviceStatus.Value == TestStatus.Failed)
                {
                    throw new TestInjectorClientException("Error failed: " + exceptionDetails.Message);
                }

                if (serviceStatus.Value == TestStatus.Exception)
                {
                    throw TransformException(exceptionDetails);
                }
            }
        }

        protected virtual Exception TransformException(ExceptionDetails exceptionDetails)
        {
            return new TestInjectorInvokeException(exceptionDetails);
        }
    }
}
