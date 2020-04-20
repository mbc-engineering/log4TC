using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mbc.Log4Tc.Dispatcher.DispatchExpression
{
    public static class IDispatchExpressionExtensions
    {
        public static IServiceCollection AddLog4TcDispatchExpression(this IServiceCollection services, IDispatchExpression dispatchExpression)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDispatchExpression>(dispatchExpression));
            return services;
        }
    }
}
