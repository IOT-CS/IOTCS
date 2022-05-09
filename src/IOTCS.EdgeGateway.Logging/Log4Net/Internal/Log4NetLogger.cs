using System;
using System.IO;

namespace IOTCS.EdgeGateway.Logging.Log4Net.Internal
{
    /// <summary>
    /// Log4Net 日志操作
    /// </summary>
    internal class Log4NetLogger : ILogger
    {
        /// <summary>
        /// Log4Net 日志操作
        /// </summary>
        private readonly log4net.ILog _log;        

        /// <summary>
        /// 当前定义类型
        /// </summary>
        private static readonly Type ThisDeclaringType = typeof(Log4NetLogger);

        /// <summary>
        /// 初始化一个<see cref="Log4NetLogger"/>类型的实例
        /// </summary>
        /// <param name="log">Log4Net 日志操作</param>
        public Log4NetLogger(log4net.ILog log)
        {
            _log = log;            
        }

        /// <summary>
        /// Log4Net 日志仓储
        /// </summary>
        internal static log4net.Repository.ILoggerRepository Repository { get; set; }

        /// <summary>
        /// 仓储名称
        /// </summary>
        internal static string RepositoryName { get; set; }

        /// <summary>
        /// 初始化日志仓储
        /// </summary>
        /// <param name="repositoryName">仓储名称</param>
        /// <param name="configFile">配置文件</param>
        internal static void InitRepository(string repositoryName, string configFile)
        {
            var log4netPath = string.Empty;
            RepositoryName = repositoryName;
            if (Repository == null)
            {
                Repository = log4net.LogManager.CreateRepository(repositoryName);
                log4netPath = configFile.GetFullPath();                
                log4net.Config.XmlConfigurator.ConfigureAndWatch(Repository, new FileInfo(log4netPath));                
            }
        }

        private static bool ModifyLog4NetPath(log4net.Repository.ILoggerRepository repository)
        {
            var appenders = repository.GetAppenders();
            log4net.Appender.RollingFileAppender targetApder = null;
            foreach (var appender in appenders)
            {
                targetApder = appender as log4net.Appender.RollingFileAppender;
                if (targetApder != null)
                {
                    targetApder.File = targetApder.File.GetFullPath();
                    targetApder.ActivateOptions();
                }
            }

            return true;
        }

        /// <summary>
        /// 检查给定日志级别是否启用
        /// </summary>
        /// <param name="level">日志级别</param>
        public bool IsEnabled(LogLevel level) => _log.Logger.IsEnabledFor(GetLevel(level));

        /// <summary>
        /// 获取日志级别
        /// </summary>
        /// <param name="level">日志级别</param>
        private log4net.Core.Level GetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return log4net.Core.Level.Trace;
                case LogLevel.Debug:
                    return log4net.Core.Level.Debug;
                case LogLevel.Information:
                    return log4net.Core.Level.Info;
                case LogLevel.Warning:
                    return log4net.Core.Level.Warn;
                case LogLevel.Error:
                    return log4net.Core.Level.Error;
                case LogLevel.Fatal:
                    return log4net.Core.Level.Fatal;
                default:
                    return log4net.Core.Level.Off;
            }
        }

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
            _log.Logger.Log(ThisDeclaringType, GetLevel(level), message, exception);
        }
    }
}
