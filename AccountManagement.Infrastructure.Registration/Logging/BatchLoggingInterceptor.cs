using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public sealed class BatchLoggingInterceptor : BaseLoggingInterceptor
    {
        private readonly string _jobName;

        public BatchLoggingInterceptor(ILogger<BatchLoggingInterceptor> logger, string jobName,
            IOptions<LoggingOptions> options)
            : base(logger, "Batch", options)
        {
            _jobName = jobName;
        }

        protected override void LogEntry(IInvocation invocation, string className, string methodName)
        {
            var entry = new BatchEntry("ENTRY", _jobName, className, methodName, BuildArgs(invocation));
            Log.Information("{@BatchEntry}", entry);
        }

        protected override void LogExit(IInvocation invocation, string className, string methodName, long durationMs, object? result)
        {
            var exit = new BatchExit("EXIT", _jobName, className, methodName, durationMs, result);
            Log.Information("{@BatchExit}", exit);
        }

        protected override void LogError(IInvocation invocation, string className, string methodName, long durationMs, Exception ex)
        {
            var error = new BatchError("ERROR", _jobName, className, methodName, durationMs, CleanException(ex));
            Log.Error("{@BatchError}", error);
        }
    }

    public record BatchEntry(string Direction, string JobName, string Class, string Method, object Args);
    public record BatchExit(string Direction, string JobName, string Class, string Method, long DurationMs, object? Result);
    public record BatchError(string Direction, string JobName, string Class, string Method, long DurationMs, string Exception);
}
