using Autofac;
using Log4Tc.Output.NLog;
using Log4Tc.Receiver;
using Mbc.Common.Service;
using System;

namespace Log4Tc.Service
{
    class Program
    {
        private static ServiceStartableManager _serviceStartableManager;

        static void Main(string[] args)
        {
            var a = new AdsLogReceiver();
            var b = new NLogLog4TcOutput(a);

            a.Start();
            b.Start();

            Console.ReadKey();

            b.Stop();
            a.Stop();

            a.Dispose();
            return;

            var builder = new ContainerBuilder();
            builder.RegisterType<AdsLogReceiver>().SingleInstance();
            builder.RegisterType<NLogLog4TcOutput>().SingleInstance();

            var container = builder.Build();

            _serviceStartableManager = new ServiceStartableManager(container);

            _serviceStartableManager.StartStartableComponents();

            Console.ReadKey();

            _serviceStartableManager.StopStartableComponents();

            container.Dispose();
        }
    }
}
