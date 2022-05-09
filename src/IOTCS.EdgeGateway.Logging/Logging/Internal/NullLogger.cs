using System;

namespace IOTCS.EdgeGateway.Logging.Internal
{
    /// <summary>
    /// 空日志操作
    /// </summary>
    internal class NullLogger : ILogger
    {
        /// <summary>
        /// 空日志操作实例
        /// </summary>
        internal static NullLogger Instance = new NullLogger();

        /// <summary>
        /// 检查给定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        public bool IsEnabled(LogLevel level) => false;

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public void Log(LogLevel level, string message, Exception exception)
        {
        }
    }
}
