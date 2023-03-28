using System;

namespace PatHead.PubSub.Core.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class ScanItemResModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ISubAttribute SubAttribute { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Type Proxy { get; set; }
    }
}