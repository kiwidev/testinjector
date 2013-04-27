// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace TestInjector.Client
{
    public class TestInjectorClientException : Exception
    {
        public TestInjectorClientException(string message, Exception innerException = null)
            :base(message, innerException)
        {
            
        }


    }
}
