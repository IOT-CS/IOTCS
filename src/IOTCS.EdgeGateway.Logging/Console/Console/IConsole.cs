using System;

namespace IOTCS.EdgeGateway.Logging.Console
{
    /// <summary>
    /// 控制台
    /// </summary>
    internal interface IConsole
    {
        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        void Write(string message, ConsoleColor? background, ConsoleColor? foreground);

        /// <summary>
        /// 写入并换行
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground);

        /// <summary>
        /// 清除缓冲区，并将缓冲数据写入
        /// </summary>
        void Flush();
    }
}
