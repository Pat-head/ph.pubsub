using Microsoft.Extensions.Options;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisProvider
    /// </summary>
    public class RedisProvider
    {
        private readonly RedisPubSubOption _options;

        private CSRedis.CSRedisClient _csRedisClient;

        /// <summary>
        /// RedisProvider
        /// </summary>
        /// <param name="options"></param>
        public RedisProvider(IOptions<RedisPubSubOption> options)
        {
            _options = options.Value;
            InitRedisConn();
        }

        private void InitRedisConn()
        {
            var connectionString = _options.RedisConnectionString;
            _csRedisClient = new CSRedis.CSRedisClient(connectionString);
        }

        /// <summary>
        /// GetClient
        /// </summary>
        /// <returns></returns>
        public CSRedis.CSRedisClient GetClient()
        {
            return _csRedisClient;
        }
    }
}