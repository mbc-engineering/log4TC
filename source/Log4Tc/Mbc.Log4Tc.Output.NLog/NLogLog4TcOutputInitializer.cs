using Mbc.Log4Tc.Model;
using NLog;
using NLog.Config;
using System;
using System.IO;

namespace Mbc.Log4Tc.Output.NLog
{
    public static class NLogLog4TcOutputInitializer
    {
        public static bool SetupDone { get; private set; }

        public static void Setup()
        {
            if (!SetupDone)
            {
                var path = Path.Combine(OsPaths.GetConfigBasePath(), "NLog.config");
                LogManager.Configuration = new XmlLoggingConfiguration(path);
                SetupDone = true;
            }
        }
    }
}
