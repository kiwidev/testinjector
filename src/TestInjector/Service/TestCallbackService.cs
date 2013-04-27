namespace TestInjector.Service
{
    public class TestCallbackService : ITestCallbackService
    {
        public TestDetails GetMethodToRun()
        {
            return ServiceController.GetCurrentTestDetails();
        }

        public void ReportTestProgress(string testName, TestStatus status, ExceptionDetails exceptionDetails, string traceDetails)
        {
            ServiceController.InvokeCurrentCallback(testName, status, exceptionDetails, traceDetails);
        }
    }
}
