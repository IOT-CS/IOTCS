using System;
using System.Text;

namespace IOTCS.EdgeGateway.Logging.Console.Internal
{
    /// <summary>
    /// Unix 控制台日志
    /// </summary>
    internal class UnixLogConsole : IConsole
    {
        /// <summary>
        /// 消息构建器
        /// </summary>
        private readonly StringBuilder _messageBuilder = new StringBuilder();

        /// <summary>
        /// 默认背景色
        /// </summary>
        private const string DefaultBackground = "\x1B[39m\x1B[22m";

        /// <summary>
        /// 默认前景色
        /// </summary>
        private const string DefaultForeground = "\x1B[49m";

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            if (background.HasValue)
                _messageBuilder.Append(GetBackgroundColor(background.Value));
            if (foreground.HasValue)
                _messageBuilder.Append(GetForegroundColor(foreground.Value));
            _messageBuilder.Append(message);
            if (foreground.HasValue)
                _messageBuilder.Append(DefaultForeground);
            if (background.HasValue)
                _messageBuilder.Append(DefaultBackground);
        }

        /// <summary>
        /// 写入并换行
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            Write(message, background, foreground);
            _messageBuilder.AppendLine();
        }

        /// <summary>
        /// 清除缓冲区，并将缓冲数据写入
        /// </summary>
        public void Flush()
        {
            System.Console.Write(_messageBuilder.ToString());
            _messageBuilder.Clear();
        }

        /// <summary>
        /// 获取前景色
        /// </summary>
        /// <param name="color">颜色</param>
        private static string GetForegroundColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[30m";
                case ConsoleColor.DarkRed:
                    return "\x1B[31m";
                case ConsoleColor.DarkGreen:
                    return "\x1B[32m";
                case ConsoleColor.DarkYellow:
                    return "\x1B[33m";
                case ConsoleColor.DarkBlue:
                    return "\x1B[34m";
                case ConsoleColor.DarkMagenta:
                    return "\x1B[35m";
                case ConsoleColor.DarkCyan:
                    return "\x1B[36m";
                case ConsoleColor.Gray:
                    return "\x1B[37m";
                case ConsoleColor.Red:
                    return "\x1B[1m\x1B[31m";
                case ConsoleColor.Green:
                    return "\x1B[1m\x1B[32m";
                case ConsoleColor.Yellow:
                    return "\x1B[1m\x1B[33m";
                case ConsoleColor.Blue:
                    return "\x1B[1m\x1B[34m";
                case ConsoleColor.Magenta:
                    return "\x1B[1m\x1B[35m";
                case ConsoleColor.Cyan:
                    return "\x1B[1m\x1B[36m";
                case ConsoleColor.White:
                    return "\x1B[1m\x1B[37m";
                default:
                    return DefaultForeground;
            }
        }

        /// <summary>
        /// 获取背景色
        /// </summary>
        /// <param name="color">颜色</param>
        private static string GetBackgroundColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "\x1B[40m";
                case ConsoleColor.Red:
                    return "\x1B[41m";
                case ConsoleColor.Green:
                    return "\x1B[42m";
                case ConsoleColor.Yellow:
                    return "\x1B[43m";
                case ConsoleColor.Blue:
                    return "\x1B[44m";
                case ConsoleColor.Magenta:
                    return "\x1B[45m";
                case ConsoleColor.Cyan:
                    return "\x1B[46m";
                case ConsoleColor.White:
                    return "\x1B[47m";
                default:
                    return DefaultBackground;
            }
        }
    }
}
