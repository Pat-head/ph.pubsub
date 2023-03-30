using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;
using PatHead.PubSub.Redis.RunContainers;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisPubSubFactory
    /// </summary>
    public class RedisPubSubFactory : AbstractPubSubFactory
    {
        private readonly ILogger<RedisPubSubFactory> _logger;
        private readonly RedisPubSubOption _options;

        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public RedisPubSubFactory(IServiceProvider serviceProvider,
            ILogger<RedisPubSubFactory> logger,
            IOptions<RedisPubSubOption> options) : base(serviceProvider)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void PrintStatus()
        {
            var asd = _tasks;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Current Status:");
            builder.AppendLine($"Run Queue: {_tasks.Count}");
            _logger.LogInformation(builder.ToString());
        }

        /// <summary>
        /// Start
        /// </summary>
        public override void Start()
        {
            var scanResModel = Scan<RedisSubAttribute>(_options.RegisterAssembly);

            foreach (var item in scanResModel.Items)
            {
                var task = Task.Run(async () =>
                {
                    var redisSubAttribute = (RedisSubAttribute)item.SubAttribute;
                    var instance = (ISub)ActivatorUtilities.CreateInstance(this.ServiceProvider, item.Proxy);
                    var redisProvider = this.ServiceProvider.GetRequiredService<RedisProvider>();
                    var key = GenerateKey(redisSubAttribute);

                    if (redisSubAttribute.Persistence && redisSubAttribute.Multicast)
                    {
                        var container = new MpQueueRunContainer(key, instance, redisProvider);
                        await container.Run();
                    }
                    else if (redisSubAttribute.Multicast)
                    {
                        var container = new MQueueRunContainer(key, instance, redisProvider);
                        await container.Run();
                    }
                    else
                    {
                        var container = new QueueRunContainer(key, instance, redisProvider);
                        await container.Run();
                    }
                });
                _tasks.Add(task);
            }
        }

        private string GenerateKey(RedisSubAttribute redisSubAttribute)
        {
            var environment = "default";
            if (!string.IsNullOrWhiteSpace(_options.Environment))
            {
                environment = _options.Environment;
            }

            var prefix = "prefix";
            if (!string.IsNullOrWhiteSpace(redisSubAttribute.Prefix))
            {
                prefix = redisSubAttribute.Prefix;
            }

            return $"{environment}:{prefix}:{redisSubAttribute.Key}";
        }
    }
}