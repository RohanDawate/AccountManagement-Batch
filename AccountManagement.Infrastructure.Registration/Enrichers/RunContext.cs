using Serilog.Context;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public static class RunContext
    {
        public static string ContextType { get; private set; } = string.Empty;
        public static string RunId { get; private set; } = string.Empty;

        public static void Initialize(string contextType)
        {
            ContextType = contextType;

            var identifier = new Random().Next(10000, 99999); // 5-digit
            var date = DateTimeOffset.Now.ToString("yyyyMMdd");
            var time = DateTimeOffset.Now.ToString("HHmmss");

            RunId = $"{contextType}_{identifier}_{date}_{time}";

            LogContext.PushProperty("ContextType", ContextType);
            LogContext.PushProperty("RunId", RunId);
        }
    }
}
