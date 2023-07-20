using System;
using System.Threading.Tasks;

namespace PatHead.PubSub.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISubRunContainer
    {
        int Count { get; set; }
        bool IsCancel { get; set; } 

        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        Task Run();

        /// <summary>
        /// Stop
        /// </summary>
        /// <returns></returns>
        Task Stop();
    }
}