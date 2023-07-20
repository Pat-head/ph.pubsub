using System;
using System.Text;
using System.Threading.Tasks;
using PatHead.PubSub.Core;
using PatHead.PubSub.Redis;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PatHead.PubSub.RabbitMq.RunContainers
{
    /// <summary>
    /// 持久化广播队列
    /// </summary>
    public class MbQueueRunContainer : ISubRunContainer
    {
        private readonly ISub _sub;
        private readonly RabbitMqProvider _rabbitMqProvider;
        private readonly string _key;

        /// <summary>
        /// RedisSubRunContainer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sub"></param>
        /// <param name="rabbitMqProvider"></param>
        public MbQueueRunContainer(string key, ISub sub, RabbitMqProvider rabbitMqProvider)
        {
            _key = key;
            _sub = sub;
            _rabbitMqProvider = rabbitMqProvider;
        }

        public int Count { get; set; }

        public bool IsCancel { get; set; }
        
        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        public Task Run()
        {
            var random = Guid.NewGuid().ToString("N");

            var connection = _rabbitMqProvider.GetClient();

            var channel = connection.CreateModel();

            channel.ExchangeDeclare(_key, ExchangeType.Fanout);

            var queueName = $"{_key}_{random}";
            channel.QueueDeclare(queueName, false, false, false, null);

            channel.QueueBind(queueName, _key, "");

            channel.BasicQos(0, 1, false);

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    Count++;
                    _sub.Handler(message).GetAwaiter().GetResult();
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    channel.BasicReject(ea.DeliveryTag, false);
                }
            };

            //启动消费者, 设置为手动应答消息
            channel.BasicConsume(queue: queueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}