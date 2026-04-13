using Serilog.Core;
using Serilog.Events;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class GlobalContextEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // Add RunContext (from your static class)
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ContextType", RunContext.ContextType));

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "RunId", RunContext.RunId));

            // If the static TraceId has a value, add it to every log record
            if (!string.IsNullOrEmpty(TraceContext.TraceId))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", TraceContext.TraceId));
            }

            if (!string.IsNullOrEmpty(TraceContext.IdentifierName))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RecIdentifier", TraceContext.IdentifierName));
            }

            if (!string.IsNullOrEmpty(TraceContext.IdentifierValue))
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RecValue", TraceContext.IdentifierValue));
            }

        }
    }
}
