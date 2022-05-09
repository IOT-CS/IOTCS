using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace IOTCS.EdgeGateway.Logging.Console.Internal
{
    /// <summary>
    /// 控制台日志操作
    /// </summary>
    internal class ConsoleLogger : ILogger
    {
        /// <summary>
        /// 控制台
        /// </summary>
        private readonly IConsole _console;

        /// <summary>
        /// 日志名称
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 最小日志级别
        /// </summary>
        private readonly LogLevel _minLevel;

        /// <summary>
        /// 默认控制台颜色
        /// </summary>
        private readonly ConsoleColor? _defaultConsoleColor = ConsoleColor.White;

        /// <summary>
        /// 输出队列
        /// </summary>
        private readonly OutputQueue _outputQueue;

        /// <summary>
        /// 初始化一个<see cref="ConsoleLogger"/>类型的实例
        /// </summary>
        /// <param name="name">日志名称</param>
        /// <param name="minLevel">最小日志级别</param>
        public ConsoleLogger(string name, LogLevel minLevel)
        {
            _name = name;
            _minLevel = minLevel;
            _console = GetConsole();
            _outputQueue = new OutputQueue(_console);
        }

        /// <summary>
        /// 获取控制台
        /// </summary>
        private static IConsole GetConsole()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsLogConsole();
            return new UnixLogConsole();
        }

        /// <summary>
        /// 是否启用颜色
        /// </summary>
        public bool ColorEnabled { get; set; } = true;

        /// <summary>
        /// 检查给定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        public bool IsEnabled(LogLevel level) => level >= _minLevel;

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public void Log(LogLevel level, string message, Exception exception)
        {
            if (!IsEnabled(level))
                return;
            message = Formatter(message, exception);
            if (string.IsNullOrWhiteSpace(message))
                return;
            WriteLine(level, _name, message);
        }

        /// <summary>
        /// 写入并换行
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="name">日志名称</param>
        /// <param name="message">日志消息</param>
        protected virtual void WriteLine(LogLevel level, string name, string message)
        {
            var color = ColorEnabled ? GetColor(level) : new Color(_defaultConsoleColor, _defaultConsoleColor);
            var levelStr = GetLevelString(level);
            _outputQueue.EnqueueMessage(new LogMessage()
            {
                Message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {levelStr} {name} {message}",
                Background = color.Background,
                Foreground = color.Foreground
            });
        }

        /// <summary>
        /// 获取日志级别字符串
        /// </summary>
        /// <param name="level">日志级别</param>
        private static string GetLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "trace";
                case LogLevel.Debug:
                    return "debug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "error";
                case LogLevel.Fatal:
                    return "fatal";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <param name="level">日志级别</param>
        private Color GetColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return new Color(ConsoleColor.Magenta, ConsoleColor.Black);
                case LogLevel.Error:
                    return new Color(ConsoleColor.Red, ConsoleColor.Black);
                case LogLevel.Warning:
                    return new Color(ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Information:
                    return new Color(ConsoleColor.White, ConsoleColor.Black);
                case LogLevel.Trace:
                    return new Color(ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Debug:
                    return new Color(ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    return new Color(ConsoleColor.White, ConsoleColor.Black);
            }
        }

        /// <summary>
        /// 颜色
        /// </summary>
        private struct Color
        {
            /// <summary>
            /// 初始化一个<see cref="Color"/>类型的实例
            /// </summary>
            /// <param name="foreground">前景色</param>
            /// <param name="background">背景色</param>
            public Color(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            /// <summary>
            /// 前景色
            /// </summary>
            public ConsoleColor? Foreground { get; }

            /// <summary>
            /// 背景色
            /// </summary>
            public ConsoleColor? Background { get; }
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="error">异常信息</param>
        private static string Formatter(string message, Exception error)
        {
            if (message == null && error == null)
                throw new InvalidOperationException("找不到日志消息或异常信息来写入日志消息");
            if (message == null)
                return error.ToString();
            if (error == null)
                return message;
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", message, Environment.NewLine,
                error.ToString());
        }
    }
}
