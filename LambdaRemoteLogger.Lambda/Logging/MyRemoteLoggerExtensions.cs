using Microsoft.Extensions.Options;

namespace LambdaRemoteLogger.Lambda.Logging
{
    public static class MyRemoteLoggerExtensions
    {
        public static ILoggingBuilder AddRemoteLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, RemoteLoggerProvider>();
            return builder;
        }
    }
}
