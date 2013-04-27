// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;

namespace TestInjector
{
    public class ApplicationLaunchException : Exception
    {
        public ApplicationLaunchException(Exception inner)
            :this("Failed to launch application. Please see inner exception for details", inner)
        {
            
        }

        public ApplicationLaunchException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
