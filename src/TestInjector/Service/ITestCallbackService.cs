using System.Runtime.Serialization;
using System.ServiceModel;

namespace TestInjector.Service
{
    [DataContract]
    public enum TestStatus
    {
        [EnumMember] Started,
        [EnumMember] Failed,
        [EnumMember] Exception,
        [EnumMember] Success
    }

    [DataContract]
    public class ExceptionDetails
    {
        [DataMember]
        public string ExceptionType { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ExceptionString { get; set; }
    }

    [DataContract]
    public class AssemblyRef
    {
        [DataMember]
        public string AssemblyName { get; set; }

        [DataMember]
        public string FileName { get; set; }
    }

    [DataContract]
    public class TestDetails
    {
        [DataMember]
        public AssemblyRef[] AssembliesToLoad { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public string MethodName { get; set; }

        [DataMember]
        public object[] Params { get; set; }
    }

    [ServiceContract]
    public interface ITestCallbackService
    {
        [OperationContract]
        TestDetails GetMethodToRun();

        [OperationContract]
        void ReportTestProgress(string testName, TestStatus status, ExceptionDetails exceptionDetails, string traceDetails);
    }
}
