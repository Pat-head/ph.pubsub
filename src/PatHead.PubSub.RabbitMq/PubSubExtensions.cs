using System;
using Microsoft.Extensions.DependencyInjection;

namespace PatHead.PubSub.RabbitMq
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
        public static IServiceCollection AddRabbitMqPubSub(
            this IServiceCollection services,
            Action<RabbitMqPubSubOption> options)
        {
            services.Configure(options);
            services.AddSingleton<RabbitMqPubSubFactory>();
            services.AddSingleton<RabbitMqPublisher>();
            services.AddSingleton<RabbitMqProvider>();
            return services;
        }
    }
}