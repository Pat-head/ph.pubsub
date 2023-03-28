using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatHead.PubSub.Core;
using PatHead.PubSub.Redis;
using Xunit.Abstractions;

namespace PatHead.PubSub.Test
{
    public class TestBase
    {
        protected IServiceProvider ServiceProvider;

        private readonly ITestOutputHelper _testOutputHelper;

        public TestBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            CreateServiceCollection();
        }

        private void CreateServiceCollection()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddRedisPubSub(option =>
            {
                var redisPubSubOption = option;
                redisPubSubOption.RedisConnectionString = "localhost:6379";
                redisPubSubOption.RegisterAssembly.Add("PatHead.PubSub.Test");
            });

            ServiceProvider = services.BuildServiceProvider();
            var redisPubSubFactory = ServiceProvider.GetService<RedisPubSubFactory>();
            redisPubSubFactory.Run();
        }
    }
}