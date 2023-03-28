using System.Collections.Generic;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisPubSubOption
    /// </summary>
    public class RedisPubSubOption : IPubSubOption
    {
        /// <summary>
        /// Environment
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Endpoint
        /// </summary>
        public string RedisConnectionString { get; set; }

        /// <summary>
        /// RegisterAssembly
        /// </summary>
        public List<string> RegisterAssembly { get; set; } = new List<string>();
    }
}