using System;
using System.Threading.Tasks;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// MpQueueRunContainer
    /// </summary>
    public class QueueRunContainer : ISubRunContainer
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
        public QueueRunContainer(string key, ISub sub, RedisProvider redisProvider)
        {
            _key = key;
            _sub = sub;
            _redisProvider = redisProvider;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            while (true)
            {
                try
                {
                    var queuePop =
                        _redisProvider.GetClient()
                            .BRPop<string>(5, _key);
                    
                    if (queuePop != null)
                    {
                        await _sub.Handler(queuePop).ConfigureAwait(false);
                    }
                    else
                    {
                        TimeSpan span = TimeSpan.FromSeconds(10);
                        await Task.Delay(span).ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    TimeSpan span = TimeSpan.FromMilliseconds(100);
                    await Task.Delay(span).ConfigureAwait(false);
                }
            }
        }
    }
}