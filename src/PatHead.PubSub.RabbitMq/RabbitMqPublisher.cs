using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Redis;
using RabbitMQ.Client;

namespace PatHead.PubSub.RabbitMq
{
    /// <summary>
    /// RedisPublisher
    /// </summary>
    public class RabbitMqPublisher
    {
        private readonly RabbitMqProvider _rabbitMqProvider;
        private readonly RabbitMqPubSubOption _options;

        /// <summary>
        /// RedisPublisher
        /// </summary>
        /// <param name="rabbitMqProvider"></param>
        /// <param name="options"></param>
        public RabbitMqPublisher(RabbitMqProvider rabbitMqProvider, IOptions<RabbitMqPubSubOption> options)
        {
            _rabbitMqProvider = rabbitMqProvider;
            _options = options.Value;
        }

        /// <summary>
        /// 发送一条持久化并多方争抢的消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishAsync(string key, string prefix, string message)
        {
            var finalKey = Generator.GenerateKey(key, true, true, prefix, _options.Environment);
            var model = _rabbitMqProvider.GetClient().CreateModel();
            var body = Encoding.UTF8.GetBytes(message);
            model.BasicPublish(string.Empty, finalKey, false, null, body);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 发送一条持久化并广播的消息, 高并发下漏数据，高并发广播详细建议使用rabbitmq
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishBroadcastAsync(string key, string prefix, string message)
        {
            var finalKey = Generator.GenerateKey(key, true, false, prefix, _options.Environment);
            var channel = _rabbitMqProvider.GetClient().CreateModel();
            channel.ExchangeDeclare(finalKey, ExchangeType.Fanout);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(finalKey, "", false, null, body);
            return Task.CompletedTask;
        }
    }
}