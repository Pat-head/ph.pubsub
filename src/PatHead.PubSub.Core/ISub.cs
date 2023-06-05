using System;
using System.Threading.Tasks;

namespace PatHead.PubSub.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task Handler(Object body);
    }
}