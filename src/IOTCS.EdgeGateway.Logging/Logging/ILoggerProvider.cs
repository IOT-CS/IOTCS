using System;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志提供程序
    /// </summary>
    public interface ILoggerProvider : IDisposable
    {
        /// <summary>
        /// 创建一个新的<see cref="ILogger"/>实例
        /// </summary>
        /// <param name="name">日志名称</param>
        ILogger CreateLogger(string name);
    }
}
