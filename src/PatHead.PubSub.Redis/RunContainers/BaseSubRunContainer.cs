using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis.RunContainers
{
    /// <summary>
    /// RedisBaseSubRunContainer
    /// </summary>
    public abstract class RedisBaseSubRunContainer : ISubRunContainer
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly string Key;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// 
        /// </summary>
        protected readonly CancellationTokenSource TokenSource;


        /// <summary>
        /// 
        /// </summary>
        public bool IsCancel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected RedisBaseSubRunContainer(string key, ILoggerFactory loggerFactory)
        {
            Key = key;
            Logger = loggerFactory.CreateLogger($"redis_queue:{Key}");
            TokenSource = new CancellationTokenSource();
        }

        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public async Task Run()
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (true)
                    {
                        await LoopExecute();
                    }
                }
                catch (TaskCanceledException e)
                {
                    IsCancel = true;
                }
                catch (Exception e)
                {
                    Logger.LogCritical("database 后台线程启动失败,异常信息：{errMsg},{LogDate}",
                        e.ToString(), DateTime.UtcNow);
                    throw;
                }
            }, TokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Stop()
        {
            TokenSource.Cancel();
            return Task.CompletedTask;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Task LoopExecute();
    }
}