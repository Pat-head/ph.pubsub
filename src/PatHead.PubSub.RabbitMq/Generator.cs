namespace PatHead.PubSub.Redis
{
    /// <summary>
    /// 生成器
    /// </summary>
    public static class Generator
    {
        /// <summary>
        /// 生成Key
        /// </summary>
        /// <param name="key">key名称</param>
        /// /// <param name="persistence">是否持久化</param>
        /// <param name="seize">是否争抢</param>
        /// <param name="prefix">前缀名称</param>
        /// <param name="environment">环境名称</param>
        /// <returns></returns>
        public static string GenerateKey(string key,
            bool persistence,
            bool seize,
            string prefix = "prefix",
            string environment = "default")
        {
            var persistencePrefix = persistence ? "persistence" : "noPersistence";
            
            var seizePrefix = seize ? "seize" : "noSeize";
            
            return $"{environment}:{prefix}:{persistencePrefix}:{seizePrefix}:{key}";
        }
    }
}