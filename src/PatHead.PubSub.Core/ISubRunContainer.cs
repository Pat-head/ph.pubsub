using System.Threading.Tasks;

namespace PatHead.PubSub.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISubRunContainer
    {
        /// <summary>
        /// Run
        /// </summary>
        /// <returns></returns>
        Task Run();
    }
}