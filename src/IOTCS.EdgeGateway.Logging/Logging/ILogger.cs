using System;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志操作
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 检查给定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        void Log(LogLevel level, string message, Exception exception);
    }
}
