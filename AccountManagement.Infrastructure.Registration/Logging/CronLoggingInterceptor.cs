using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public sealed class CronLoggingInterceptor : BaseLoggingInterceptor
    {
        private readonly string _jobName;

        public CronLoggingInterceptor(ILogger<CronLoggingInterceptor> logger, string jobName,
            IOptions<LoggingOptions> options)
            : base(logger, "Cron", options)
        {
            _jobName = jobName;
        }

        protected override void LogEntry(IInvocation invocation, string className, string methodName)
        {
            var entry = new CronEntry("ENTRY", _jobName, className, methodName, BuildArgs(invocation));
            Log.Information("{@CronEntry}", entry);
        }

        protected override void LogExit(IInvocation invocation, string className, string methodName, long durationMs, object? result)
        {
            var exit = new CronExit("EXIT", _jobName, className, methodName, durationMs, result);
            Log.Information("{@CronExit}", exit);
        }

        protected override void LogError(IInvocation invocation, string className, string methodName, long durationMs, Exception ex)
        {
            var error = new CronError("ERROR", _jobName, className, methodName, durationMs, CleanException(ex));
            Log.Error("{@CronError}", error);
        }
    }

    public record CronEntry(string Direction, string JobName, string Class, string Method, object Args);
    public record CronExit(string Direction, string JobName, string Class, string Method, long DurationMs, object? Result);
    public record CronError(string Direction, string JobName, string Class, string Method, long DurationMs, string Exception);
}
