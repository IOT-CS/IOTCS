using IOTCS.EdgeGateway.Logging.Console.Internal;

namespace IOTCS.EdgeGateway.Logging.Console
{
    /// <summary>
    /// 控制台日志提供程序
    /// </summary>
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// 最小日志级别
        /// </summary>
        private readonly LogLevel _minLevel;

        /// <summary>
        /// 是否启用颜色
        /// </summary>
        private readonly bool _colorEnabled;

        /// <summary>
        /// 初始化一个<see cref="ConsoleLoggerProvider"/>类型的实例
        /// </summary>
        /// <param name="minLevel">最小日志级别</param>
        /// <param name="colorEnabled">是否启用颜色</param>
        public ConsoleLoggerProvider(LogLevel minLevel, bool colorEnabled = true)
        {
            _minLevel = minLevel;
            _colorEnabled = colorEnabled;
        }

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
        public ILogger CreateLogger(string name) => new ConsoleLogger(name, _minLevel) { ColorEnabled = _colorEnabled };
    }
}
