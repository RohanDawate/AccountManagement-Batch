using Serilog.Context;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public static class TraceContext
    {
        private static readonly AsyncLocal<string?> _traceId = new();
        private static readonly AsyncLocal<string?> _idName = new();
        private static readonly AsyncLocal<string?> _idValue = new();

        public static string TraceId => _traceId.Value ?? string.Empty;
        public static string IdentifierName => _idName.Value ?? string.Empty;
        public static string IdentifierValue => _idValue.Value ?? string.Empty;

        public static IDisposable BeginScope(string? traceId = null, string? idName = null, string? idValue = null)
        {
            var prevId = _traceId.Value;
            var prevName = _idName.Value;
            var prevVal = _idValue.Value;

            // If null is passed, use the helper to generate a default one
            var effectiveId = string.IsNullOrWhiteSpace(traceId)
                ? TraceIdHelper.Generate("GEN")
                : traceId;

            _traceId.Value = effectiveId;
            _idName.Value = idName;
            _idValue.Value = idValue;

            var logContext = LogContext.PushProperty("TraceId", traceId);

            // Return a combined disposal to reset AsyncLocals
            return new CombinedDisposable(logContext, () => {
                _traceId.Value = prevId;
                _idName.Value = prevName;
                _idValue.Value = prevVal;
            });
        }
    }
}