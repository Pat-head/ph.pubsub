using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;
using PatHead.PubSub.Core.Model;
using PatHead.PubSub.RabbitMq.RunContainers;
using PatHead.PubSub.Redis;

namespace PatHead.PubSub.RabbitMq
{
    /// <summary>
    /// RabbitMqPubSubFactory
    /// </summary>
    public class RabbitMqPubSubFactory : AbstractPubSubFactory
    {
        private readonly ILogger<RabbitMqPubSubFactory> _logger;
        private readonly RabbitMqPubSubOption _options;

        private Dictionary<string, TaskRunInfo> _tasks = new Dictionary<string, TaskRunInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public RabbitMqPubSubFactory(IServiceProvider serviceProvider,
            ILogger<RabbitMqPubSubFactory> logger,
            IOptions<RabbitMqPubSubOption> options) : base(serviceProvider)
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
            var scanResModel = Scan<RabbitMqSubAttribute>(_options.RegisterAssembly);

            foreach (var item in scanResModel.Items)
            {
                var redisSubAttribute = (RabbitMqSubAttribute)item.SubAttribute;
                var task = CreateRunContainer(item);
                var subRunContainer = task.GetAwaiter().GetResult();
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
            var redisSubAttribute = (RabbitMqSubAttribute)item.SubAttribute;

            var instance = (ISub)ActivatorUtilities.CreateInstance(this.ServiceProvider, item.Proxy);

            var redisProvider = this.ServiceProvider.GetRequiredService<RabbitMqProvider>();

            var finalKey = Generator.GenerateKey(
                redisSubAttribute.Key,
                true,
                redisSubAttribute.Seize,
                redisSubAttribute.Prefix,
                _options.Environment);

            if (redisSubAttribute.Seize)
            {
                var container = new MsQueueRunContainer(finalKey, instance, redisProvider);
                await container.Run();
                return container;
            }
            else
            {
                var container = new MbQueueRunContainer(finalKey, instance, redisProvider);
                await container.Run();
                return container;
            }
        }
    }
}