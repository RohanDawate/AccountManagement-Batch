using Serilog.Context;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public static class TraceContext
    {
        private static readonly AsyncLocal<string?> _traceId = new();

        public static string TraceId
        {
            get => _traceId.Value ?? string.Empty;
            private set => _traceId.Value = value;
        }

        public static IDisposable BeginScope(string? traceId = null)
        {
            // If null is passed, use the helper to generate a default one
            var effectiveId = string.IsNullOrWhiteSpace(traceId)
                ? TraceIdHelper.Generate("GEN")
                : traceId;

            TraceId = effectiveId;

            // PushProperty returns an IDisposable that removes the property when the 'using' block ends
            return LogContext.PushProperty("TraceId", effectiveId);
        }
    }
}