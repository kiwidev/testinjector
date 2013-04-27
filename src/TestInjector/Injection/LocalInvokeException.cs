// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Linq.Expressions;
using System.Linq;
using System;

namespace TestInjector.Injection
{
    public class LocalInvokeException : Exception
    {
        public LocalInvokeException(string message)
            :base(message)
        {
            

        }
    }
}