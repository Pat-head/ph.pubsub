using System;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Redis;
using RabbitMQ.Client;

namespace PatHead.PubSub.RabbitMq
{
    /// <summary>
    /// RedisProvider
    /// </summary>
    public class RabbitMqProvider
    {
        private readonly RabbitMqPubSubOption _options;

        private IConnection _rabbitMqConnection;

        /// <summary>
        /// RedisProvider
        /// </summary>
        /// <param name="options"></param>
        public RabbitMqProvider(IOptions<RabbitMqPubSubOption> options)
        {
            _options = options.Value;
            InitRedisConn();
        }

        private void InitRedisConn()
        {
            var connectionString = _options.RabbitMqConnectionString;

            ConnectionFactory factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            _rabbitMqConnection = factory.CreateConnection();
        }

        /// <summary>
        /// GetClient
        /// </summary>
        /// <returns></returns>
        public IConnection GetClient()
        {
            return _rabbitMqConnection;
        }
    }
}