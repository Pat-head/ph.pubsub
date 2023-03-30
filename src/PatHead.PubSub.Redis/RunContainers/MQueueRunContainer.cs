using System.Threading.Tasks;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// MQueueRunContainer
    /// </summary>
    public class MQueueRunContainer : ISubRunContainer
    {
        private readonly ISub _sub;
        private readonly RedisProvider _redisProvider;
        private readonly string _key;

        /// <summary>
        /// RedisSubRunContainer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sub"></param>
        /// <param name="redisProvider"></param>
        public MQueueRunContainer(string key, ISub sub, RedisProvider redisProvider)
        {
            _key = key;
            _sub = sub;
            _redisProvider = redisProvider;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
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