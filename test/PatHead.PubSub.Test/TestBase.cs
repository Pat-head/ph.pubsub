using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatHead.PubSub.Core;
using PatHead.PubSub.RabbitMq;
using PatHead.PubSub.Redis;
using Xunit.Abstractions;

namespace PatHead.PubSub.Test
{
    public class TestBase
    {
        protected IServiceProvider ServiceProvider;

        private readonly ITestOutputHelper _testOutputHelper;

        protected TestBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            CreateServiceCollection();
        }

        private void CreateServiceCollection()
        {
            var configurationBuilder = new ConfigurationBuilder();

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddLogging();

            services.AddRedisPubSub(option =>

            {
                var redisPubSubOption = option;
                redisPubSubOption.RedisConnectionString = "localhost:36388";
                redisPubSubOption.RegisterAssembly.Add("PatHead.PubSub.Test");
            });

            services.AddRabbitMqPubSub(option =>
            {
                var redisPubSubOption = option;
                redisPubSubOption.RabbitMqConnectionString = "amqp://admin:admin@172.16.127.100:27074";
                redisPubSubOption.RegisterAssembly.Add("PatHead.PubSub.Test");
                redisPubSubOption.Environment = "test";
            });

            ServiceProvider = services.BuildServiceProvider();
            var redisPubSubFactory = ServiceProvider.GetService<RedisPubSubFactory>();
            var rabbitMqPubSubFactory = ServiceProvider.GetService<RabbitMqPubSubFactory>();
            redisPubSubFactory.Start();
            rabbitMqPubSubFactory.Start();
        }
    }
}