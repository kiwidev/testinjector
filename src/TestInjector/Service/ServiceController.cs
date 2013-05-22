using System;
using System.ServiceModel;

namespace TestInjector.Service
{
    public static class ServiceController
    {
        public const string ServiceAddress = "net.pipe://localhost/TestInjector/CallbackEndpoint";

        public delegate void TestCallbackHandler(string testName, TestStatus status, ExceptionDetails exceptionDetails, string traceLog);

        private static IDisposable _currentService = null;
        private static TestCallbackHandler _currentCallback;
        private static TestDetails _testDetails;

        public static IDisposable StartService(TestDetails testDetails, TestCallbackHandler testCallback)
        {
            if (_currentService != null)
            {
                _currentService.Dispose();
                _currentService = null;
            }

            _currentCallback = testCallback;
            _testDetails = testDetails;

            ServiceHost host = new ServiceHost(typeof(TestCallbackService));
            try
            {
                var binding = new NetNamedPipeBinding();
                binding.MaxReceivedMessageSize = 100000;
                host.AddServiceEndpoint(typeof(ITestCallbackService), binding, new Uri(ServiceAddress));
                host.Open();
            }
            catch (Exception)
            {
                ((IDisposable)host).Dispose();
                _currentCallback = null;
                throw;
            }
            return new ServiceWrapper(host);
        }

        private class ServiceWrapper : IDisposable
        {
            private readonly ServiceHost _host;
            private bool _disposed;

            public ServiceWrapper(ServiceHost host)
            {
                _host = host;
            }

            public void WaitForUpdate()
            {
                // Use threads to wait here...
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _currentCallback = null;

                _disposed = true;

                _host.Close();
                ((IDisposable)_host).Dispose();
            }
        }

        public static void InvokeCurrentCallback(string testName, TestStatus status, ExceptionDetails exceptionDetails, string traceLog)
        {
            if (_currentCallback == null)
                return;

            _currentCallback(testName, status, exceptionDetails, traceLog);
        }

        public static TestDetails GetCurrentTestDetails()
        {
            if (OnTestDetailsRetrieved != null)
                OnTestDetailsRetrieved();

            return _testDetails;
        }

        public static Action OnTestDetailsRetrieved { get; set; }
    }
}
