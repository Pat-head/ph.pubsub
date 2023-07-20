using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;
using PatHead.PubSub.Core.Model;
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

        private Dictionary<string, TaskRunInfo> _tasks = new Dictionary<string, TaskRunInfo>();

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
                var redisSubAttribute = (RedisSubAttribute)item.SubAttribute;
                var subRunContainer = CreateRunContainer(item).GetAwaiter().GetResult();
                var taskRun = new TaskRunInfo(subRunContainer);
                _tasks.Add(redisSubAttribute.Name, taskRun);
            }
        }

        /// <summary>
        /// GetSubRunContainer
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISubRunContainer GetSubRunContainer(string name)
        {
            if (_tasks.TryGetValue(name, out var taskRunInfo))
            {
                return taskRunInfo.SubRunContainer;
            }

            return null;
        }

        private async Task<ISubRunContainer> CreateRunContainer(ScanItemResModel item)
        {
            var redisSubAttribute = (RedisSubAttribute)item.SubAttribute;

            var instance = (ISub)ActivatorUtilities.CreateInstance(this.ServiceProvider, item.Proxy);

            var redisProvider = this.ServiceProvider.GetRequiredService<RedisProvider>();

            var loggerFactory = this.ServiceProvider.GetRequiredService<ILoggerFactory>();

            var finalKey = Generator.GenerateKey(
                redisSubAttribute.Key,
                true,
                redisSubAttribute.Seize,
                redisSubAttribute.Prefix,
                _options.Environment);

            if (redisSubAttribute.Seize)
            {
                var container = new MsQueueRunContainer(finalKey, instance, redisProvider, loggerFactory);
                await container.Run();
                return container;
            }
            else
            {
                var container = new MbQueueRunContainer(finalKey, instance, redisProvider, loggerFactory);
                await container.Run();
                return container;
            }
        }
    }
}