﻿using System;
using System.Security.Principal;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisSubAttribute : Attribute, ISubAttribute
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// 争抢模式
        /// </summary>
        public bool Seize { get; private set; } = false;

        /// <summary>
        /// RedisSubAttribute
        /// </summary>
        public RedisSubAttribute(string name, string key, string prefix = null, bool seize = false)
        {
            Name = name;
            Key = key;
            Prefix = prefix;
            Seize = seize;
        }
    }
}