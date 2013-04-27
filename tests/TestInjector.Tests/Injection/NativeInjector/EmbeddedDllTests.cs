// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestInjector.Injection.NativeInjector;

namespace TestInjector.Tests.Injection.NativeInjector
{
    [TestClass]
    public class EmbeddedDllTests
    {
        [TestMethod]
        public void Ensure32BitDllCanBeExtracted()
        {
            EnsureDllCanBeExtracted(InjectorMethods.InjectMethodNativeFunctions.DllName32);
        }

        [TestMethod]
        public void Ensure64BitDllCanBeExtracted()
        {
            EnsureDllCanBeExtracted(InjectorMethods.InjectMethodNativeFunctions.DllName64);
        }

        private void EnsureDllCanBeExtracted(string dllName)
        {
            // Check to see if the file is there
            var expectedFile =
                Path.Combine(
                    Path.GetDirectoryName(typeof(EmbeddedDllTests).Assembly.Location),
                    dllName
                    );

            if (File.Exists(expectedFile))
            {
                File.Delete(expectedFile);
            }

            InjectorMethods.EnsureFile(dllName);

            if (!File.Exists(expectedFile))
            {
                Assert.Fail("Expected file \'" + expectedFile + "\' to be created but couldn't find it");
            }
        }
    }
}
