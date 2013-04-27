// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ServiceModel;
using System.ServiceModel.Channels;
using System;
using TestInjector.Service;

namespace TestInjector.Client
{
    public class TestCallbackClient : ClientBase<ITestCallbackService>, ITestCallbackService, IDisposable
    {
        public static TestCallbackClient Create()
        {
            var binding = new NetNamedPipeBinding();

            var endpointAddress = new EndpointAddress(ServiceController.ServiceAddress);

            return new TestCallbackClient(binding, endpointAddress);
        }

        public TestCallbackClient(Binding binding, EndpointAddress endpointAddress)
            :base(binding, endpointAddress)
        {
            
        }

        public TestDetails GetMethodToRun()
        {
            return base.Channel.GetMethodToRun();
        }

        public void ReportTestProgress(string testName, TestStatus status, ExceptionDetails exceptionDetails, string traceDetails)
        {
            base.Channel.ReportTestProgress(testName, status, exceptionDetails, traceDetails);
        }
    }
}
