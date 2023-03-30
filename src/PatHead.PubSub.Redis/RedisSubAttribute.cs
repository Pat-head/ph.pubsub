using System;
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
        /// Prefix
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Persistence
        /// </summary>
        public bool Persistence { get; private set; } = false;

        /// <summary>
        /// Multicast
        /// </summary>
        public bool Multicast { get; private set; } = false;

        /// <summary>
        /// RedisSubAttribute
        /// </summary>
        public RedisSubAttribute(string key, string prefix = null, bool persistence = false, bool multicast = false)
        {
            Key = key;
            Prefix = prefix;
            Persistence = persistence;
            Multicast = multicast;
        }
    }
}