using LinkTamer.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LinkTamer.Infrastructure.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string redisConnectionString)
        {
            services.AddSingleton(new RedisConnection(redisConnectionString));
            services.AddSingleton<IConnectionMultiplexer>(sp => RedisConnection.Connection);
            return services;
        }
    }
}
