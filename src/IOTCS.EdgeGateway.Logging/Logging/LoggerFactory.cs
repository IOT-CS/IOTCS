using System.Collections.Generic;
using IOTCS.EdgeGateway.Logging.Internal;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public class LoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// 对象锁
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// 日志操作字典
        /// </summary>
        internal readonly IDictionary<string, AggregateLogger> Loggers = new Dictionary<string, AggregateLogger>();

        /// <summary>
        /// 默认日志工厂
        /// </summary>
        public static readonly ILoggerFactory Factory = new LoggerFactory();

        /// <summary>
        /// 日志提供程序列表
        /// </summary>
        public readonly List<ILoggerProvider> Providers = new List<ILoggerProvider>()
        {
            NullLoggerProvider.Instance
        };

        /// <summary>
        /// 创建日志操作实例
        /// </summary>
        /// <param name="name">日志名称</param>
        public ILogger CreateLogger(string name)
        {
            if (!Loggers.TryGetValue(name, out var logger))
            {
                lock (_lock)
                {
                    if (!Loggers.TryGetValue(name, out logger))
                    {
                        logger = new AggregateLogger(Providers.ToArray(), name);
                        Loggers[name] = logger;
                    }
                }
            }
            return logger;
        }

        /// <summary>
        /// 添加日志提供程序
        /// </summary>
        /// <param name="providers">日志提供程序列表</param>
        public void AddProvider(params ILoggerProvider[] providers)
        {
            lock (_lock)
            {
                // 添加日志提供程序时，将已创建的日志操作集合追加到新的日志提供程序
                // 这样我们可以确保每个日志操作实例包含完整多个不同的实现
                Providers.AddRange(providers);
                foreach (var logger in Loggers)
                    logger.Value.AddProvider(providers);
            }
        }

        /// <summary>
        /// 获取当前类日志操作实例
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public static ILogger GetCurrentClassLogger()
        {
            System.Diagnostics.StackFrame frame = new System.Diagnostics.StackFrame(1, false);
            return Factory.CreateLogger(frame.GetMethod().DeclaringType.FullName);
        }

        /// <summary>
        /// 获取日志操作实例
        /// </summary>
        /// <param name="name">日志名称</param>
        public static ILogger GetLogger(string name) => Factory.CreateLogger(name);

        /// <summary>
        /// 获取日志操作实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public static ILogger GetLogger<T>() => Factory.CreateLogger<T>();
    }
}
