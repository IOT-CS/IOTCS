using System;
using System.Collections.Generic;

namespace IOTCS.EdgeGateway.Logging.Internal
{
    /// <summary>
    /// 聚合日志操作
    /// </summary>
    internal class AggregateLogger : ILogger
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 日志操作列表
        /// </summary>
        internal List<ILogger> Loggers = new List<ILogger>();

        public AggregateLogger(ILoggerProvider[] providers, string name)
        {
            _name = name;
            AddProvider(providers);
        }

        /// <summary>
        /// 添加日志提供程序。如果当前日志记录器已经创建，后期可能会追加新的<see cref="ILoggerProvider"/>并且根据日志名创建不同的日志操作
        /// </summary>
        /// <param name="providers">日志提供程序列表</param>
        internal void AddProvider(ILoggerProvider[] providers)
        {
            foreach (var provider in providers)
                Loggers.Add(provider.CreateLogger(_name));
        }

        /// <summary>
        /// 检查给定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        public bool IsEnabled(LogLevel level)
        {
            List<Exception> exceptions = null;
            foreach (var logger in Loggers)
            {
                try
                {
                    if (logger.IsEnabled(level))
                        return true;
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                        exceptions = new List<Exception>();
                    exceptions.Add(e);
                }
            }
            if (exceptions != null && exceptions.Count > 0)
                throw new AggregateException("An error occurred while writing to logger(s).", exceptions);
            return false;
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志消息</param>
        /// <param name="exception">异常信息</param>
        public void Log(LogLevel level, string message, Exception exception)
        {
            List<Exception> exceptions = null;
            foreach (var logger in Loggers)
            {
                try
                {
                    logger.Log(level, message, exception);
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                        exceptions = new List<Exception>();
                    exceptions.Add(e);
                }
            }
            if (exceptions != null && exceptions.Count > 0)
                throw new AggregateException("An error occurred while writing to logger(s).", exceptions);
        }
    }
}
