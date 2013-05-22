// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq.Expressions;
using System;
using System.Linq;

namespace TestInjector.Injection
{
    public class InjectionException : Exception
    {
        public InjectionException(string message)
            :base(message)
        {
            
        }

        public InjectionException(string message, Exception innerException)
            :base(message, innerException)
        {
                
        }
    }
}
