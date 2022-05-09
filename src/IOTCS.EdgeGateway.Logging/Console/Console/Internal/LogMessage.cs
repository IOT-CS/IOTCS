using System;

namespace IOTCS.EdgeGateway.Logging.Console.Internal
{
    /// <summary>
    /// 日志消息
    /// </summary>
    internal class LogMessage
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public ConsoleColor? Background { get; set; }

        /// <summary>
        /// 前景色
        /// </summary>
        public ConsoleColor? Foreground { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
