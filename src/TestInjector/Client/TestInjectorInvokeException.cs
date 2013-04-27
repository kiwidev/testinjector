// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using TestInjector.Service;

namespace TestInjector.Client
{

    public class TestInjectorInvokeException : Exception
    {
        private readonly ExceptionDetails _exceptionDetails;

        public TestInjectorInvokeException(ExceptionDetails exceptionDetails)
            : base("Error returned from client invocation: " + exceptionDetails.Message,
                   new Exception(exceptionDetails.ExceptionString))
        {
            _exceptionDetails = exceptionDetails;
        }

        public ExceptionDetails ExceptionDetails
        {
            get { return _exceptionDetails; }
        }
    }
}
