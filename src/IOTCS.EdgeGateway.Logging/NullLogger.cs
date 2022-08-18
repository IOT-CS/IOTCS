namespace IOTCS.EdgeGateway.Logging
{
    public class NullLogger
    {
        public static ILogger Logger
        {
            get
            {
                var loggerFactory = new LoggerFactory()
                 .AddConsole(LogLevel.Trace, true)
                 .AddLog4Net();
                var logger = loggerFactory.CreateLogger("errorAppender");
                return logger;
            }
        }
    }
}
