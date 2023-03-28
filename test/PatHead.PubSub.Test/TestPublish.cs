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
            while (true)
            {
                Thread.Sleep(100);
                await service.PublishAsync("key", null, "oh");
            }
        }
    }


    [RedisSub(Key = "key")]
    public class SubService : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"{body}");
            return Task.CompletedTask;
        }
    }
}