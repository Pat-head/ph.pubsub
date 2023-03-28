using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    public class RedisPublisher
    {
        private readonly RedisProvider _redisProvider;
        private readonly RedisPubSubOption _options;

        public RedisPublisher(RedisProvider redisProvider, IOptions<RedisPubSubOption> options)
        {
            _redisProvider = redisProvider;
            _options = options.Value;
        }

        public Task PublishAsync(string key, string prefix, string message)
        {
            var finalKey = GenerateKey(key, prefix);
            return _redisProvider.GetClient().PublishAsync(finalKey, message);
        }

        private string GenerateKey(string key, string prefix)
        {
            var environment = "default";
            if (!string.IsNullOrWhiteSpace(_options.Environment))
            {
                environment = _options.Environment;
            }

            var finalPrefix = "prefix";
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                finalPrefix = prefix;
            }

            return $"{environment}:{finalPrefix}:{key}";
        }
    }
}