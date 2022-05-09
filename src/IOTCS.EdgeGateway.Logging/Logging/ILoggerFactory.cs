namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建日志操作实例
        /// </summary>
        /// <param name="name">日志名称</param>
        ILogger CreateLogger(string name);

        /// <summary>
        /// 添加日志提供程序
        /// </summary>
        /// <param name="providers">日志提供程序列表</param>
        void AddProvider(params ILoggerProvider[] providers);
    }
}
