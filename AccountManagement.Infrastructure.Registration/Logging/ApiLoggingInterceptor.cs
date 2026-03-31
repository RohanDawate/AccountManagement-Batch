using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public sealed class ApiLoggingInterceptor : BaseLoggingInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiLoggingInterceptor(ILogger<ApiLoggingInterceptor> logger, IHttpContextAccessor accessor,
            IOptions<LoggingOptions> options)
            : base(logger, "API", options)
        {
            _httpContextAccessor = accessor;
        }

        protected override void LogEntry(IInvocation invocation, string className, string methodName)
        {
            var entry = new ApiEntry("ENTRY", className, methodName, BuildArgs(invocation),
                                     _httpContextAccessor.HttpContext?.TraceIdentifier);
            Log.Information("{@ApiEntry}", entry);
        }

        protected override void LogExit(IInvocation invocation, string className, string methodName, long durationMs, object? result)
        {
            var exit = new ApiExit("EXIT", className, methodName, durationMs, result, _httpContextAccessor.HttpContext?.TraceIdentifier);
            Log.Information("{@ApiExit}", exit);
        }

        protected override void LogError(IInvocation invocation, string className, string methodName, long durationMs, Exception ex)
        {
            var error = new ApiError("ERROR", className, methodName, durationMs, CleanException(ex), _httpContextAccessor.HttpContext?.TraceIdentifier);
            Log.Error("{@ApiError}", error);
        }
    }

    public record ApiEntry(string Direction, string Class, string Method, object Args, string? RequestId);
    public record ApiExit(string Direction, string Class, string Method, long DurationMs, object? Result, string? RequestId);
    public record ApiError(string Direction, string Class, string Method, long DurationMs, string Exception, string? RequestId);

}
