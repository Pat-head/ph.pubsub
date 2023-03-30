using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PatHead.PubSub.Core;
using PatHead.PubSub.Core.Model;
using PatHead.PubSub.Redis;
using Xunit;
using Xunit.Abstractions;

namespace PatHead.PubSub.Test
{
    public class TestPublish : TestBase
    {
        public TestPublish(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task TestPublish1()
        {
            var service = this.ServiceProvider.GetService<RedisPublisher>();
            
            var redisPubSubFactory = ServiceProvider.GetService<RedisPubSubFactory>();
            
            for (int i = 0; i < 1000000; i++)
            {
                Thread.Sleep(100);
                await service.PublishMpAsync("key", null, $"This is my {i} message;");
                redisPubSubFactory.PrintStatus();
            }
        }
    }

    [RedisSub("key", multicast: true, persistence: true)]
    public class SubPersistenceService1 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am Judy ,{body}");
            return Task.CompletedTask;
        }
    }

    [RedisSub("key", multicast: true, persistence: true)]
    public class SubPersistenceService2 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am John ,{body}");
            return Task.CompletedTask;
        }
    }

    [RedisSub("key", multicast: true, persistence: true)]
    public class SubPersistenceService3 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am Naveen,{body}");
            return Task.CompletedTask;
        }
    }
}