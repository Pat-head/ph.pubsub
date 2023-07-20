using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// 持久化争抢队列
    /// </summary>
    public class MsQueueRunContainer : RedisBaseSubRunContainer
    {
        private readonly ISub _sub;
        private readonly RedisProvider _redisProvider;

        /// <summary>
        /// RedisSubRunContainer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sub"></param>
        /// <param name="redisProvider"></param>
        /// <param name="factory">ILoggerFactory</param>
        public MsQueueRunContainer(string key,
            ISub sub,
            RedisProvider redisProvider,
            ILoggerFactory factory) : base(key, factory)
        {
            _sub = sub;
            _redisProvider = redisProvider;
        }

        /// <summary>
        /// LoopExecute
        /// </summary>
        protected override async Task LoopExecute()
        {
            Thread.Sleep(10);
            try
            {
                var queuePop =
                    _redisProvider.GetClient().BRPop(5, Key);

                if (queuePop != null)
                {
                    Count++;
                    _sub.Handler(queuePop).GetAwaiter().GetResult();
                }
                else
                {
                    TimeSpan span = TimeSpan.FromSeconds(10);
                    await Task.Delay(span, TokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Logger.LogCritical("database 后台处理redis队列失败,异常信息：{errMsg},{LogDate}",
                    e.ToString(), DateTime.UtcNow);
                TimeSpan span = TimeSpan.FromMilliseconds(100);
                await Task.Delay(span, TokenSource.Token).ConfigureAwait(false);
            }
        }
    }
}