using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public sealed class DefaultLoggingInterceptor : BaseLoggingInterceptor
    {
        public DefaultLoggingInterceptor(ILogger<DefaultLoggingInterceptor> logger,
            IOptions<LoggingOptions> options)
            : base(logger, "Default", options) { }

        protected override void LogEntry(IInvocation invocation, string className, string methodName)
        {
            var entry = new DefaultEntry(className, methodName, BuildArgs(invocation));
            Log.Information("{@DefaultEntry}", entry);
        }

        protected override void LogExit(IInvocation invocation, string className, string methodName, long durationMs, object? result)
        {
            var exit = new DefaultExit(className, methodName, durationMs, result);
            Log.Information("{@DefaultExit}", exit);
        }

        protected override void LogError(IInvocation invocation, string className, string methodName, long durationMs, Exception ex)
        {
            var error = new DefaultError(className, methodName, durationMs, CleanException(ex));
            Log.Error("{@DefaultError}", error);
        }
    }

    public record DefaultEntry(string Class, string Method, object Args);
    public record DefaultExit(string Class, string Method, long DurationMs, object? Result);
    public record DefaultError(string Class, string Method, long DurationMs, string Exception);

}
