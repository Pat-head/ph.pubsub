using System.Collections.Generic;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.RabbitMq
{
    /// <summary>
    /// RedisPubSubOption
    /// </summary>
    public class RabbitMqPubSubOption : IPubSubOption
    {
        /// <summary>
        /// 环境名称，用以前缀区分
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// RabbitMq链接字符串 "amqp://user:pass@hostName:port/vhost"
        /// </summary>
        public string RabbitMqConnectionString { get; set; }

        /// <summary>
        /// 扫描的订阅程序集
        /// </summary>
        public List<string> RegisterAssembly { get; set; } = new List<string>();
    }
}