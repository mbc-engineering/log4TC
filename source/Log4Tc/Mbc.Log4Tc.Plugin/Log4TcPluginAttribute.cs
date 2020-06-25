using System;

namespace Mbc.Log4Tc.Plugin
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class Log4TcPluginAttribute : Attribute
    {
        public Log4TcPluginAttribute()
        {
        }
    }
}
