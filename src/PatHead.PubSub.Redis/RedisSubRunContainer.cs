using System.Threading.Tasks;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    public class RedisSubRunContainer : ISubRunContainer
    {
        private readonly ISub _sub;
        private readonly RedisProvider _redisProvider;
        private readonly string _key;

        public RedisSubRunContainer(string key, ISub sub, RedisProvider redisProvider)
        {
            _key = key;
            _sub = sub;
            _redisProvider = redisProvider;
        }

        public Task Run()
        {
            _redisProvider.GetClient().Subscribe((_key, msg =>
            {
                var objBody = msg.Body;
                _sub.Handler(objBody).GetAwaiter().GetResult();
            }));
            return Task.CompletedTask;
        }
    }
}