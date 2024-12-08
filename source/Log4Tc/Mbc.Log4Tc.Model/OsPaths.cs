using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mbc.Log4Tc.Model
{
    public static class OsPaths
    {
        public static string GetConfigBasePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.ExpandEnvironmentVariables("%programdata%"), "log4TC", "config");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "/etc/log4tc/config";
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows and linux system supported.");
            }
        }

        public static string GetInternalLogBasePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.ExpandEnvironmentVariables("%programdata%"), "log4TC", "internal");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "/var/log/log4tc";
            }
            else
            {
                throw new PlatformNotSupportedException("Service still in windows and linux system supported.");
            }
        }
    }
}
