using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LinkTamer.Infrastructure.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string redisConnectionString)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
               ConnectionMultiplexer.Connect(redisConnectionString));
            return services;
        }
    }
}
