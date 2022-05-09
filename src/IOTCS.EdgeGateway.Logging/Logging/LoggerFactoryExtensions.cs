namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志工厂(<see cref="ILoggerFactory"/>) 扩展
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        /// <summary>
        /// 创建日志操作实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="factory">日志工厂</param>
        public static ILogger CreateLogger<T>(this ILoggerFactory factory) => factory.CreateLogger(typeof(T).FullName);
    }
}
