using IOTCS.EdgeGateway.Logging.Console;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 控制台日志工厂(<see cref="ILoggerFactory"/>) 扩展
    /// </summary>
    public static class ConsoleLoggerFactoryExtensions
    {
        /// <summary>
        /// 添加控制台输出记录器
        /// </summary>
        /// <param name="factory">日志工厂</param>
        /// <param name="minLevel">最小日志级别</param>
        /// <param name="colorEnabled">是否启用颜色</param>
        public static ILoggerFactory AddConsole(this ILoggerFactory factory, LogLevel minLevel, bool colorEnabled = true)
        {
            factory.AddProvider(new ConsoleLoggerProvider(minLevel, colorEnabled));
            return factory;
        }
    }
}
