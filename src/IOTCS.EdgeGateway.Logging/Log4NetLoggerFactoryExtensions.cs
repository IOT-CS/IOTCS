using IOTCS.EdgeGateway.Logging.Log4Net;
using IOTCS.EdgeGateway.Logging.Log4Net.Internal;

namespace IOTCS.EdgeGateway.Logging
{
    /// <summary>
    /// Log4Net日志工厂(<see cref="ILoggerFactory"/>) 扩展
    /// </summary>
    internal static class Log4NetLoggerFactoryExtensions
    {
        /// <summary>
        /// 添加Log4Net日志记录器
        /// </summary>
        /// <param name="factory">日志工厂</param>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory) => AddLog4Net(factory, InternalConst.DefaultLogName, InternalConst.DefaultConfigFile);

        /// <summary>
        /// 添加Log4Net日志记录器
        /// </summary>
        /// <param name="factory">日志工厂</param>
        /// <param name="configFile">配置文件</param>
        /// <returns></returns>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configFile) => AddLog4Net(factory, InternalConst.DefaultLogName, configFile);

        /// <summary>
        /// 添加Log4Net日志记录器
        /// </summary>
        /// <param name="factory">日志工厂</param>
        /// <param name="repositoryName">仓储名称</param>
        /// <param name="configFile">配置文件</param>
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string repositoryName, string configFile)
        {
            factory.AddProvider(new Log4NetLoggerProvider());
            Log4NetLogger.InitRepository(repositoryName, configFile);
            return factory;
        }
    }
}
