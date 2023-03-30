using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisPublisher
    /// </summary>
    public class RedisPublisher
    {
        private readonly RedisProvider _redisProvider;
        private readonly RedisPubSubOption _options;

        /// <summary>
        /// RedisPublisher
        /// </summary>
        /// <param name="redisProvider"></param>
        /// <param name="options"></param>
        public RedisPublisher(RedisProvider redisProvider, IOptions<RedisPubSubOption> options)
        {
            _redisProvider = redisProvider;
            _options = options.Value;
        }

        /// <summary>
        /// send message
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishAsync(string key, string prefix, string message)
        {
            var finalKey = GenerateKey(key, prefix);
            return _redisProvider.GetClient().LPushAsync(finalKey, message);
        }

        /// <summary>
        /// send multicast message
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishMAsync(string key, string prefix, string message)
        {
            var finalKey = GenerateKey(key, prefix);
            return _redisProvider.GetClient().PublishAsync(finalKey, message);
        }

        /// <summary>
        /// send persistent multicast messages
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishMpAsync(string key, string prefix, string message)
        {
            var finalKey = GenerateKey(key, prefix);
            return _redisProvider.GetClient().LPushAsync(finalKey, message);
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