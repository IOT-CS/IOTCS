using System;

namespace IOTCS.EdgeGateway.Logging.Console.Internal
{
    /// <summary>
    /// Windows 控制台日志
    /// </summary>
    internal class WindowsLogConsole : IConsole
    {
        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        public void Write(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            SetColor(background, foreground);
            System.Console.Out.Write(message);
            ResetColor();
        }

        /// <summary>
        /// 写入并换行
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        public void WriteLine(string message, ConsoleColor? background, ConsoleColor? foreground)
        {
            SetColor(background, foreground);
            System.Console.Out.WriteLine(message);
            ResetColor();
        }

        /// <summary>
        /// 清除缓冲区，并将缓冲数据写入
        /// </summary>
        public void Flush()
        {
            // 什么都不用做，数据直接发送到控制台
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="background">背景色</param>
        /// <param name="foreground">前景色</param>
        private void SetColor(ConsoleColor? background, ConsoleColor? foreground)
        {
            if (background.HasValue)
                System.Console.BackgroundColor = background.Value;
            if (foreground.HasValue)
                System.Console.ForegroundColor = foreground.Value;
        }

        /// <summary>
        /// 重置颜色
        /// </summary>
        private void ResetColor() => System.Console.ResetColor();
    }
}
