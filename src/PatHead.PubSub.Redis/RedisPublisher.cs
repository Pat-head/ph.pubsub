﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisPublisher
    /// </summary>
    public class RedisPublisher
    {
        private readonly RedisProvider _redisProvider;
        private readonly RedisPubSubOption _options;

        /// <summary>
        /// RedisPublisher
        /// </summary>
        /// <param name="redisProvider"></param>
        /// <param name="options"></param>
        public RedisPublisher(RedisProvider redisProvider, IOptions<RedisPubSubOption> options)
        {
            _redisProvider = redisProvider;
            _options = options.Value;
        }

        /// <summary>
        /// 发送一条持久化并多方争抢的消息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishAsync(string key, string prefix, string message)
        {
            var finalKey = Generator.GenerateKey(key, true, true, prefix, _options.Environment);
            return _redisProvider.GetClient().LPushAsync(finalKey, message);
        }

        /// <summary>
        /// 发送一条持久化并广播的消息, 高并发下漏数据，高并发广播详细建议使用rabbitmq
        /// </summary>
        /// <param name="key"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task PublishBroadcastAsync(string key, string prefix, string message)
        {
            var finalKey = Generator.GenerateKey(key, true, false, prefix, _options.Environment);
            return _redisProvider.GetClient().LPushAsync(finalKey, message);
        }
    }
}