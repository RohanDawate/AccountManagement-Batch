using Serilog.Core;
using Serilog.Events;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class GlobalContextEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // 1. Add RunContext (from your static class)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ContextType", RunContext.ContextType));

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "RunId", RunContext.RunId));

            //// 2. Add TraceId (from .NET Activity System)
            //// This automatically picks up the ID if you are using OpenTelemetry or standard Diagnostics
            //var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace-id";
            //logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            //    "TraceId", traceId));

            // If the static TraceId has a value, add it to every log record
            if (!string.IsNullOrEmpty(TraceContext.TraceId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", TraceContext.TraceId));
            }

        }
    }
}
