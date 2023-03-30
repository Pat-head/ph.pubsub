using System;
using System.Threading.Tasks;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// MpQueueRunContainer
    /// </summary>
    public class MpQueueRunContainer : ISubRunContainer
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
        public MpQueueRunContainer(string key, ISub sub, RedisProvider redisProvider)
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
            var random = Guid.NewGuid().ToString("N");
            _redisProvider.GetClient()
                .SubscribeListBroadcast(_key, random, msg => { _sub.Handler(msg).GetAwaiter().GetResult(); });
            return Task.CompletedTask;
        }
    }
}