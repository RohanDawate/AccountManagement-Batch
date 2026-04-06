using Serilog.Context;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public static class TraceContext
    {
        private static readonly AsyncLocal<string?> _traceId = new();

        public static string TraceId
        {
            get => _traceId.Value ?? string.Empty;
            set => _traceId.Value = value;
        }

        public static IDisposable BeginScope(string traceId)
        {
            TraceId = traceId;
            return LogContext.PushProperty("TraceId", traceId);
        }
    }
}