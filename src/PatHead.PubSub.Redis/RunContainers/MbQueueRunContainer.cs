using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// 持久化广播队列
    /// </summary>
    public class MbQueueRunContainer : RedisBaseSubRunContainer
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
        public MbQueueRunContainer(string key,
            ISub sub,
            RedisProvider redisProvider,
            ILoggerFactory factory) : base(key, factory)
        {
            _sub = sub;
            _redisProvider = redisProvider;
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        public Task Run()
        {
            return Task.CompletedTask;
        }


        /// <summary>
        /// LoopExecute
        /// </summary>
        protected override async Task LoopExecute()
        {
            try
            {
                var random = Guid.NewGuid().ToString("N");

                _redisProvider.GetClient()
                    .SubscribeListBroadcast(Key, random, msg =>
                    {
                        // 为空好像类似于健康检查，暂且不管
                        if (string.IsNullOrEmpty(msg))
                        {
                            return;
                        }

                        Count++;
                        _sub.Handler(msg).GetAwaiter().GetResult();
                    });
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