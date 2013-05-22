// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Linq.Expressions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TestInjector.Properties;

namespace TestInjector.Injection.NativeInjector
{
    internal static class InjectorMethods
    {
        public static string InjectMethod(IntPtr mainWindowHandle, Type type, string methodName, string parameter)
        {
            if (mainWindowHandle == IntPtr.Zero)
                throw new InvalidOperationException("mainWindowHandle cannot be NULL");

            InjectMethodNativeFunctions.InjectCodeDelegate injectCodeMethod;
            if (Environment.Is64BitProcess)
            {
                EnsureFile(InjectMethodNativeFunctions.DllName64);
                injectCodeMethod = InjectMethodNativeFunctions.InjectCode64;
            }
            else
            {
                EnsureFile(InjectMethodNativeFunctions.DllName32);
                injectCodeMethod = InjectMethodNativeFunctions.InjectCode32;
            }

            string resultLog = injectCodeMethod(
                mainWindowHandle, 
                type.Assembly.Location,
                type.FullName,
                methodName,
                parameter);

            Debug.WriteLine("Injection result: " + resultLog);
            if (resultLog == null || !resultLog.StartsWith("Success"))
            {
                throw new InjectionException(
                    string.Format(Resources.ERROR_InjectionFailed, resultLog)
                    );
            }

            return resultLog;
        }

        internal static void EnsureFile(string fileName, string sourceFileName = null)
        {
            if (sourceFileName == null)
            {
                sourceFileName = typeof(InjectorMethods).Namespace + "." + fileName;
            }

            string executingFolder = Path.GetDirectoryName(typeof(InjectorMethods).Assembly.Location);
            if (executingFolder == null)
            {
                throw new InjectionException(Resources.ERROR_CannotGetPath);
            }

            string destinationFilePath = Path.Combine(executingFolder, fileName);

            if (File.Exists(destinationFilePath))
            {
                if (IsEmbeddedFileOlderThanCurrentDll(destinationFilePath))
                {
                    try
                    {
                        File.Delete(destinationFilePath);
                    }
                    catch (IOException ex)
                    {
                        throw new InjectionException(Resources.ERROR_DeleteManagedInjectorDll, ex);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        throw new InjectionException(Resources.ERROR_DeleteManagedInjectorDll, ex);
                    }
                }
            }

            if (!File.Exists(destinationFilePath))
            {
                var embeddedStream = typeof (InjectorMethods).Assembly
                    .GetManifestResourceStream(sourceFileName);
                if (embeddedStream == null)
                {
                    throw new InjectionException(Resources.ERROR_CannotFindEmbeddedFile);
                }

                using (embeddedStream)
                {
                    using (var outputFile = File.Create(destinationFilePath))
                    {
                        embeddedStream.CopyTo(outputFile);
                    }
                }
            }
        }

        private static bool _checkedForOlder = false;

        private static bool IsEmbeddedFileOlderThanCurrentDll(string destinationFilePath)
        {
            if (_checkedForOlder)
                return false;
            
            _checkedForOlder = true;
            var destinationCreateTime = File.GetLastWriteTimeUtc(destinationFilePath);
            var currentCreateTime = File.GetLastWriteTimeUtc(typeof (InjectorMethods).Assembly.Location);

            return destinationCreateTime < currentCreateTime;
        }

        internal static class InjectMethodNativeFunctions
        {
            public const string DllName32 = "InjectMethod32.dll";
            public const string DllName64 = "InjectMethod64.dll";

            public delegate string InjectCodeDelegate(
                IntPtr windowHandle, string assembly, string className, string methodName, string methodParam);

            [DllImport(DllName32, EntryPoint = "InjectCode", CharSet = CharSet.Ansi,
                CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string InjectCode32(IntPtr windowHandle,
                                                     [MarshalAs(UnmanagedType.LPStr)] string assembly,
                                                     [MarshalAs(UnmanagedType.LPStr)] string className,
                                                     [MarshalAs(UnmanagedType.LPStr)] string methodName,
                                                     [MarshalAs(UnmanagedType.LPStr)] string methodParam
                                                    );

            [DllImport(DllName64, EntryPoint = "InjectCode", CharSet = CharSet.Ansi,
                CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string InjectCode64(IntPtr windowHandle,
                                                     [MarshalAs(UnmanagedType.LPStr)] string assembly,
                                                     [MarshalAs(UnmanagedType.LPStr)] string className,
                                                     [MarshalAs(UnmanagedType.LPStr)] string methodName,
                                                     [MarshalAs(UnmanagedType.LPStr)] string methodParam
                                                    );

        }
    }
}
