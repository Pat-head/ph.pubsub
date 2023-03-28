using System;
using Microsoft.Extensions.DependencyInjection;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public static class PubSubExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedisPubSub(
            this IServiceCollection services,
            Action<RedisPubSubOption> options)
        {
            services.Configure(options);
            services.AddSingleton<RedisPubSubFactory>();
            services.AddSingleton<RedisPublisher>();
            services.AddSingleton<RedisProvider>();
            return services;
        }
    }
}