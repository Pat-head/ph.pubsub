using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    public class RedisPubSubFactory : AbstractPubSubFactory
    {
        private readonly RedisPubSubOption _options;

        public RedisPubSubFactory(IServiceProvider serviceProvider,
            IOptions<RedisPubSubOption> options) : base(serviceProvider)
        {
            _options = options.Value;
        }

        public override void Run()
        {
            var scanResModel = Scan<RedisSubAttribute>(_options.RegisterAssembly);

            foreach (var item in scanResModel.Items)
            {
                Task.Run(() =>
                {
                    var redisSubAttribute = (RedisSubAttribute)item.SubAttribute;
                    var instance = (ISub)ActivatorUtilities.CreateInstance(this.ServiceProvider, item.Proxy);
                    var redisProvider = this.ServiceProvider.GetRequiredService<RedisProvider>();
                    var key = GenerateKey(redisSubAttribute);
                    var container = new RedisSubRunContainer(key, instance, redisProvider);
                    container.Run();
                });
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


    public class RedisProvider
    {
        private readonly RedisPubSubOption _options;

        private CSRedis.CSRedisClient _csRedisClient;

        public RedisProvider(IOptions<RedisPubSubOption> options)
        {
            _options = options.Value;
            InitRedisConn();
        }

        private void InitRedisConn()
        {
            var connectionString = _options.RedisConnectionString;
            _csRedisClient = new CSRedis.CSRedisClient(connectionString);
        }

        public CSRedis.CSRedisClient GetClient()
        {
            return _csRedisClient;
        }
    }
}