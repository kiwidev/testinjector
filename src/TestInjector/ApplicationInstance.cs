// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestInjector
{
    /// <summary>
    /// Wraps an instance of an application, making it easier to retrieve the process
    /// and manage disposal
    /// </summary>
    public class ApplicationInstance : IDisposable
    {
        private Process _process;

        public ApplicationInstance()
        {
            
        }

        /// <summary>
        /// Equivalent of calling new ApplicationInstance().Launch(location)
        /// </summary>
        /// <param name="location"></param>
        public ApplicationInstance(string location)
        {
            Launch(location);
        }

        /// <summary>
        /// Rela
        /// </summary>
        /// <param name="location">Relative based on the current executing process. Be aware that different test runners can
        /// change this location</param>
        /// <param name="applicationLaunchSettings">Settings referring to the launch. 
        /// Defaults to <see cref="ApplicationLaunchSettings.Default"/></param>
        /// <returns>File location of the actual exe launched</returns>
        public string Launch(string location, ApplicationLaunchSettings applicationLaunchSettings = null)
        {
            applicationLaunchSettings = applicationLaunchSettings ?? ApplicationLaunchSettings.Default;

            if (_process != null)
            {
                throw new InvalidOperationException("Process is already attached. Please detach before trying to launch.");
            }

            string fileLocation = location;
            StringBuilder searchPaths = new StringBuilder();
            

            if (!Path.IsPathRooted(fileLocation))
            {
                location = location.Replace("/", "\\");

                string currentDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
                if (currentDirectory == null)
                    throw new InvalidOperationException("Could not locate current directory");

                fileLocation = Path.Combine(currentDirectory, location);
                searchPaths.AppendLine(fileLocation);

                int numberOfDirectoriesToSearch = applicationLaunchSettings.NumberOfParentDirectoriesToSearchForExe;

                while (!File.Exists(fileLocation) && numberOfDirectoriesToSearch-- > 0)
                {
                    fileLocation = Path.Combine(currentDirectory, "..\\" + location);
                    searchPaths.AppendLine(fileLocation);
                }
            }
            else
            {
                searchPaths.AppendLine(fileLocation);
            }

            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException("Cannot find application to launch. Search paths searched:\r\n" +
                                                searchPaths.ToString());
            }

            StartProcess(fileLocation, applicationLaunchSettings);
            return fileLocation;
        }

        protected virtual void StartProcess(string fileLocation, ApplicationLaunchSettings applicationLaunchSettings)
        {
            try
            {
                Process process = Process.Start(fileLocation);
                if (process == null)
                    throw new ApplicationException("Process.Start returned null");

                _process = process;
                KillProcessOnDispose = true;
                
                Thread.Sleep(applicationLaunchSettings.MinimumTimeToWaitForApplicationToLaunch);

                _process.WaitForInputIdle(10000);
                int count = 500;
                while (count-- > 0 && _process.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(10);
                }

                // TODO: Throw timeout exception
            }
            catch (Exception ex)
            {
                throw new ApplicationLaunchException(ex);
            }
        }

        public void Attach(Process process)
        {
            if (_process != null)
            {
                throw new InvalidOperationException("Process is already attached. Detach process first before attaching");
            }
            
            _process = process;
            KillProcessOnDispose = false;
        }

        public void Detach()
        {
            _process = null;
            KillProcessOnDispose = false;
        }

        /// <summary>
        /// If true, will kill the process on dispose. Automatically set to true if <see cref="Launch"/> has been used,
        /// false otherwise
        /// </summary>
        public bool KillProcessOnDispose { get; set; }

        /// <summary>
        /// Returns the process currently attached to
        /// </summary>
        public Process Process
        {
            get { return _process; }
        }

       
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ApplicationInstance()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_process != null && KillProcessOnDispose)
            {
                try
                {
                    if (!_process.HasExited)
                    {
                        _process.Kill();
                        _process.WaitForExit(1000);
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationLaunchException("Failed to kill process. See inner exception for details", ex);
                }
            }
        }
    }
}
