using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PatHead.PubSub.Core;
using PatHead.PubSub.Core.Model;
using PatHead.PubSub.RabbitMq;
using PatHead.PubSub.Redis;
using Xunit;
using Xunit.Abstractions;

namespace PatHead.PubSub.Test
{
    public class TestPublishRabbitMq : TestBase, IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestPublishRabbitMq(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task TestPublishSeize()
        {
            var service = this.ServiceProvider.GetService<RabbitMqPublisher>();

            var rabbitMqPubSubFactory = ServiceProvider.GetService<RabbitMqPubSubFactory>();

            Random random = new Random();

            var seizeMessageCount = 0;
            var noSeizeMessageCount = 0;

            for (int i = 0; i < 10000; i++)
            {
                //await service.PublishAsync("seize", null, $"发送一条互斥消息{i}");
                // var next = random.Next(0, 2);
                //
                // if (next == 0)
                // {
                //     seizeMessageCount++;
                //     await service.PublishAsync("seize", null, $"发送一条互斥消息{i}");
                // }
                // else
                // {
                noSeizeMessageCount++;
                await service.PublishBroadcastAsync("noSeize", null, $"发送一条广播消息{i}");
                // }
            }

            {
                var runContainer1 = rabbitMqPubSubFactory.GetSubRunContainer("seize1");
                var runContainer2 = rabbitMqPubSubFactory.GetSubRunContainer("seize2");
                Assert.True(runContainer1.Count + runContainer2.Count == seizeMessageCount);
            }

            {
                var runContainer1 = rabbitMqPubSubFactory.GetSubRunContainer("noSeize1");
                var runContainer2 = rabbitMqPubSubFactory.GetSubRunContainer("noSeize2");
                Assert.True(runContainer1.Count == noSeizeMessageCount && runContainer2.Count == noSeizeMessageCount);
            }
        }

        public void Dispose()
        {
            Console.WriteLine("调用了方法");
        }
    }

    [RabbitMqSub("seize1", "seize", seize: true)]
    public class SubPersistenceMqService1 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am Judy ,{body}");
            return Task.CompletedTask;
        }
    }

    [RabbitMqSub("seize2", "seize", seize: true)]
    public class SubPersistenceMqService2 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am John ,{body}");
            return Task.CompletedTask;
        }
    }

    [RabbitMqSub("noSeize1", "noSeize", seize: false)]
    public class SubPersistenceMqService3 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am Naveen,{body}");
            return Task.CompletedTask;
        }
    }

    [RabbitMqSub("noSeize2", "noSeize", seize: false)]
    public class SubPersistenceMqService4 : ISub
    {
        public Task Handler(object body)
        {
            Console.WriteLine($"I am HuLi,{body}");
            return Task.CompletedTask;
        }
    }
}