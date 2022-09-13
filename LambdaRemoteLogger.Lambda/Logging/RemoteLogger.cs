namespace LambdaRemoteLogger.Lambda.Logging
{
    public class RemoteLogger : ILogger
    {
        private readonly MessageQueue messageHandler;

        public RemoteLogger(MessageQueue messageHandler)
        {
            this.messageHandler = messageHandler;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
           return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            messageHandler.AddMessage(formatter(state, exception));
        }
    }
}
