using System.ComponentModel;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 跟踪。包含最详细的日志信息，这些消息可能包含敏感的应用程序数据。这些消息都是默认禁用的，不应该在生产环境中启用。
        /// </summary>
        [Description("跟踪")]
        Trace = 0,
        /// <summary>
        /// 调试。在开发过程中日志用于交互式调查，这些日志应该主要包含有用的调试信息。
        /// </summary>
        [Description("调试")]
        Debug = 1,
        /// <summary>
        /// 信息。日志跟踪应用程序的通用流，这些日志应该长期显示。
        /// </summary>
        [Description("信息")]
        Information = 2,
        /// <summary>
        /// 警告。应用程序流的异常或意外事件，但不会导致应用程序停止执行。
        /// </summary>
        [Description("警告")]
        Warning = 3,
        /// <summary>
        /// 错误。强调在当前流的执行停止时由于失败，这些应该显示当前活动的失败，不是一个应用程序的失败。
        /// </summary>
        [Description("错误")]
        Error = 4,
        /// <summary>
        /// 致命错误。日志描述一个不可恢复的应用程序或系统崩溃，或者一个灾难性故障，需要立即注意。
        /// </summary>
        [Description("致命错误")]
        Fatal = 5,
        /// <summary>
        /// 关闭所有日志，不输出日志
        /// </summary>
        [Description("关闭日志")]
        None = 6
    }
}
