using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mbc.Log4Tc.Receiver
{
    public static class AdsLogReceiverExtensions
    {
        /// <summary>
        /// Register TwinCat 3 <see cref="AdsLogReceiver"/> as a <see cref="ILogReceiver"/>
        /// </summary>
        public static IServiceCollection AddLog4TcAdsLogReceiver(this IServiceCollection services)
        {
            services.AddSingleton<AdsLogReceiver>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILogReceiver, AdsLogReceiver>(x => x.GetRequiredService<AdsLogReceiver>()));
            services.AddHostedService<AdsLogReceiverService>();
            return services;
        }
    }
}
