// (c) Copyright Datacom Systems Ltd.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;

namespace TestInjector
{
    public class ApplicationLaunchSettings
    {
        /// <summary>
        /// Attempts to find file by trying this number of higher directories (adding ..\ to 
        /// the beginning of the application location). 0 to ignore. Only applies to relative paths
        /// </summary>
        public int NumberOfParentDirectoriesToSearchForExe { get; set; }

        public TimeSpan MinimumTimeToWaitForApplicationToLaunch { get; set; }

        public static ApplicationLaunchSettings Default
        {
            get
            {
                return new ApplicationLaunchSettings
                    {
                        NumberOfParentDirectoriesToSearchForExe = 1,
                        MinimumTimeToWaitForApplicationToLaunch = TimeSpan.FromSeconds(1)
                    };
            }
        }
    }
}
