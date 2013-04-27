// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestInjector.Client
{
    public class TestInjectorSettings
    {
        /// <summary>
        /// True if Trace.WriteLines from the application should come through to the test
        /// </summary>
        public bool CaptureTrace { get; set; }

        public static TestInjectorSettings Default
        {
            get { return new TestInjectorSettings() {CaptureTrace = true}; }
        }
    }
}
