namespace LambdaRemoteLogger.Lambda.Logging
{
    public class RemoteLoggerProvider : ILoggerProvider
    {
        private readonly MessageQueue _messageQueue;

        public RemoteLoggerProvider()
        {
            _messageQueue = new MessageQueue();
            _messageQueue.Start();
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new RemoteLogger(_messageQueue);
        }

        public void Dispose()
        {
            _messageQueue?.Stop();
        }
    }
}
