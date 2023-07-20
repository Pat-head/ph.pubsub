using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PatHead.PubSub.Core.Model;

namespace PatHead.PubSub.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractPubSubFactory
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        protected AbstractPubSubFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected static ScanResModel Scan<T>(List<string> scanSubPubAssembly)
            where T : Attribute, ISubAttribute
        {
            #region ScanAssembly

            List<ScanItemResModel> resTypes = new List<ScanItemResModel>();

            if (scanSubPubAssembly.Any())
            {
                var assemblyDic = AppDomain.CurrentDomain.GetAssemblies()
                    .ToDictionary(x => x.GetName().Name, x => x);

                foreach (var repositoryAssemblyName in scanSubPubAssembly)
                {
                    if (assemblyDic.ContainsKey(repositoryAssemblyName))
                    {
                        var assembly = assemblyDic[repositoryAssemblyName];

                        var types = assembly.GetTypes().Where(x =>
                            !x.IsGenericType &&
                            !x.IsAbstract &&
                            x.IsClass &&
                            x.GetInterface(nameof(ISub)) != null &&
                            x.GetCustomAttributes(typeof(T), false).Any()).ToList();

                        foreach (var type in types)
                        {
                            var customAttributes = type.GetCustomAttribute<T>();
                            resTypes.Add(new ScanItemResModel()
                            {
                                SubAttribute = customAttributes,
                                Proxy = type
                            });
                        }
                    }
                }
            }

            return new ScanResModel()
            {
                Items = resTypes
            };

            #endregion
        }


        /// <summary>
        /// Start
        /// </summary>
        public abstract void Start();
    }

    /// <summary>
    /// 
    /// </summary>
    public class TaskRunInfo
    {
        public ISubRunContainer SubRunContainer { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subRunContainer"></param>
        /// <param name="task"></param>
        public TaskRunInfo(ISubRunContainer subRunContainer)
        {
            SubRunContainer = subRunContainer;
        }
    }
}