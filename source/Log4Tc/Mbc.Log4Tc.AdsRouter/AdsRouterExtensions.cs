using Microsoft.Extensions.DependencyInjection;

namespace Mbc.Log4Tc.AdsRouter
{
    public static class AdsRouterExtensions
    {
        /// <summary>
        /// Register TwinCat 3 <see cref="TwinCAT.Ads.TcpRouter.AmsTcpIpRouter"/> in a hosted service
        /// </summary>
        public static IServiceCollection AddLog4TcAdsRouter(this IServiceCollection services)
        {
            services.AddHostedService<AdsRouterService>();
            return services;
        }
    }
}
