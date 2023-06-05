using System;
using System.Threading.Tasks;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// 持久化争抢队列
    /// </summary>
    public class MsQueueRunContainer : ISubRunContainer
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
        public MsQueueRunContainer(string key, ISub sub, RedisProvider redisProvider)
        {
            _key = key;
            _sub = sub;
            _redisProvider = redisProvider;
        }

        public int Count { get; set; }

        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        public Task Run()
        {
            _redisProvider.GetClient().SubscribeList(_key, msg =>
            {
                // 为空好像类似于健康检查，暂且不管
                if (string.IsNullOrEmpty(msg))
                {
                    return;
                }

                Count++;

                _sub.Handler(msg).GetAwaiter().GetResult();
            });

            return Task.CompletedTask;
        }
    }
}