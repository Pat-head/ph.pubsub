﻿using System.Collections.Generic;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// RedisPubSubOption
    /// </summary>
    public class RedisPubSubOption : IPubSubOption
    {
        /// <summary>
        /// 环境名称，用以前缀区分
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// redis链接字符串
        /// </summary>
        public string RedisConnectionString { get; set; }

        /// <summary>
        /// 扫描的订阅程序集
        /// </summary>
        public List<string> RegisterAssembly { get; set; } = new List<string>();
    }
}