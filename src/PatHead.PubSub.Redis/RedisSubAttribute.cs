using System;
using System.Security.Principal;
using PatHead.PubSub.Core;

namespace PatHead.PubSub.Redis
{
    public class RedisSubAttribute : Attribute, ISubAttribute
    {
        public string Prefix { get; set; }
        public string Key { get; set; }
    }
}