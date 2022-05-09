using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Logging.Console.Internal
{
    /// <summary>
    /// 输出队列
    /// </summary>
    internal class OutputQueue : IDisposable
    {
        /// <summary>
        /// 最大队列消息
        /// </summary>
        private const int MaxQueuedMessages = 65535;

        /// <summary>
        /// 消息队列
        /// </summary>
        private readonly BlockingCollection<LogMessage> _messageQueue = new BlockingCollection<LogMessage>(MaxQueuedMessages);

        /// <summary>
        /// 异步输出
        /// </summary>
        private readonly Task _outputTask;

        /// <summary>
        /// 控制台
        /// </summary>
        private readonly IConsole _console;

        /// <summary>
        /// 初始化一个<see cref="OutputQueue"/>类型的实例
        /// </summary>
        /// <param name="console">控制台</param>
        public OutputQueue(IConsole console)
        {
            _outputTask = Task.Factory.StartNew(ProcessQueue, this, TaskCreationOptions.LongRunning);
            _console = console;
        }

        /// <summary>
        /// 入队消息
        /// </summary>
        /// <param name="message">日志消息</param>
        internal virtual void EnqueueMessage(LogMessage message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException)
                {
                }
            }
            WriteMessage(message);
        }

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="message">日志消息</param>
        internal virtual void WriteMessage(LogMessage message)
        {
            _console.WriteLine(message.Message, message.Background, message.Foreground);
            _console.Flush();
        }

        /// <summary>
        /// 处理队列
        /// </summary>
        private void ProcessQueue()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
                WriteMessage(message);
        }

        /// <summary>
        /// 处理队列
        /// </summary>
        /// <param name="state">状态</param>
        private static void ProcessQueue(object state)
        {
            var consoleLogger = (OutputQueue) state;
            consoleLogger.ProcessQueue();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _messageQueue.CompleteAdding();
            try
            {
                _outputTask.Wait(1500);
            }
            catch
            {
                // ignored
            }
        }
    }
}
