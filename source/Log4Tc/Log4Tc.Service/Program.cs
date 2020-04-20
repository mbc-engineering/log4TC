using Autofac;
using Log4Tc.Dispatcher;
using Log4Tc.Dispatcher.DispatchExpression;
using Log4Tc.Output.NLog;
using Log4Tc.Receiver;
using Mbc.Common.Service;
using System;
using System.Collections.Generic;

namespace Log4Tc.Service
{
    class Program
    {
        private static ServiceStartableManager _serviceStartableManager;

        static void Main(string[] args)
        {
            var receiver = new AdsLogReceiver();
            var nLogOutput = new NLogLog4TcOutput("NLogOutput");
            var dispatchExpressions = new List<IDispatchExpression> { new DispatchAllLogsToOutput("NLogOutput") };
            var dispatcher = new LogDispatcher(new List<ILogReceiver> { receiver }, new List<IOutputHandler> { nLogOutput }, dispatchExpressions );

            dispatcher.Start();
            receiver.Start();

            Console.ReadKey();

            receiver.Stop();
            dispatcher.Stop();

            receiver.Dispose();
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
