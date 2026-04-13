using Serilog.Context;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public static class RunContext
    {
        public static string ContextType { get; private set; } = string.Empty;
        public static string RunId { get; private set; } = string.Empty;

        public static void Initialize(string contextType, string? identity = null)
        {
            ContextType = contextType;

            var runIdentifier = identity ?? new Random().Next(10000, 99999).ToString();
            var date = DateTimeOffset.Now.ToString("yyyyMMdd");
            var time = DateTimeOffset.Now.ToString("HHmm");

            RunId = $"{contextType}_{runIdentifier}_{date}_{time}";

            LogContext.PushProperty("ContextType", ContextType);
            LogContext.PushProperty("RunId", RunId);
        }
    }
}
