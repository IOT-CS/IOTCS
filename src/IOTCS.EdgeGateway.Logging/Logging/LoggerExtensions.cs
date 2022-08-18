using System;
using System.Globalization;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志操作(<see cref="ILogger"/>) 扩展
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 根据日志级别指定日志消息
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        private static void Logger(ILogger logger, LogLevel level, string message, Exception exception) => logger.Log(level, message, exception);

        /// <summary>
        /// 根据日志级别指定日志消息
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="level">日志级别</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        private static void Logger(ILogger logger, LogLevel level, string format, params object[] args) => logger.Log(level, string.Format(CultureInfo.InvariantCulture, format, args), null);

        /// <summary>
        /// 根据日志级别指定日志消息
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        private static void Logger(ILogger logger, LogLevel level, string message) => logger.Log(level, message, null);

        /// <summary>
        /// 输出跟踪日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Trace(this ILogger logger, string message) => Logger(logger, LogLevel.Trace, message);

        /// <summary>
        /// 输出跟踪日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Trace(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Trace, format, args);

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Debug(this ILogger logger, string message) => Logger(logger, LogLevel.Debug, message);

        /// <summary>
        /// 输出调试日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Debug(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Debug, format, args);

        /// <summary>
        /// 输出信息日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Info(this ILogger logger, string message) => Logger(logger, LogLevel.Information, message);

        /// <summary>
        /// 输出信息日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Info(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Information, format, args);

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Warn(this ILogger logger, string message) => Logger(logger, LogLevel.Warning, message);

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Warn(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Warning, format, args);

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public static void Warn(this ILogger logger, string message, Exception exception) =>
            Logger(logger, LogLevel.Warning, message, exception);

        /// <summary>
        /// 输出警告日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="exception">异常信息</param>
        public static void Warn(this ILogger logger, Exception exception) =>
            Logger(logger, LogLevel.Warning, exception.Message, exception);

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Error(this ILogger logger, string message) => Logger(logger, LogLevel.Error, message);

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Error(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Error, format, args);

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public static void Error(this ILogger logger, string message, Exception exception) =>
            Logger(logger, LogLevel.Error, message, exception);

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="exception">异常信息</param>
        public static void Error(this ILogger logger, Exception exception) =>
            Logger(logger, LogLevel.Error, exception.Message, exception);

        /// <summary>
        /// 输出致命错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        public static void Fatal(this ILogger logger, string message) => Logger(logger, LogLevel.Fatal, message);

        /// <summary>
        /// 输出致命错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="format">格式化内容</param>
        /// <param name="args">格式化参数</param>
        public static void Fatal(this ILogger logger, string format, params object[] args) =>
            Logger(logger, LogLevel.Fatal, format, args);

        /// <summary>
        /// 输出致命错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public static void Fatal(this ILogger logger, string message, Exception exception) =>
            Logger(logger, LogLevel.Fatal, message, exception);

        /// <summary>
        /// 输出致命错误日志
        /// </summary>
        /// <param name="logger">日志操作</param>
        /// <param name="exception">异常信息</param>
        public static void Fatal(this ILogger logger, Exception exception) =>
            Logger(logger, LogLevel.Fatal, exception.Message, exception);
    }
}
