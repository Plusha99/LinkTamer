using StackExchange.Redis;

namespace LinkTamer.Infrastructure.Data
{
    public class RedisConnection
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;

        public static ConnectionMultiplexer Connection => _lazyConnection.Value;

        public RedisConnection(string redisConnectionString)
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(redisConnectionString));
        }

        public IDatabase GetDatabase() => Connection.GetDatabase();
    }
}
