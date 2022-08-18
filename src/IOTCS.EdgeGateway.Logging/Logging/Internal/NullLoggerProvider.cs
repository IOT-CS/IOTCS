namespace IOTCS.EdgeGateway.Logging.Internal
{
    /// <summary>
    /// 空日志提供程序
    /// </summary>
    internal class NullLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// 空日志提供程序实例
        /// </summary>
        internal static NullLoggerProvider Instance = new NullLoggerProvider();

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 创建一个新的<see cref="ILogger"/>实例
        /// </summary>
        /// <param name="name">日志名称</param>
        public ILogger CreateLogger(string name) => NullLogger.Instance;
    }
}
