using Serilog.Core;
using Serilog.Events;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public class FlatStructureEnricher : ILogEventEnricher
    {
        private static readonly HashSet<string> _knownKeys = new()
        {
            "BatchEntry", "BatchExit", "BatchError",
            "ApiEntry", "ApiExit", "ApiError",
            "CronEntry", "CronExit", "CronError",
            "DefaultEntry", "DefaultExit", "DefaultError"
        };

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var entryKey = logEvent.Properties.Keys.FirstOrDefault(k => _knownKeys.Contains(k));
            if (entryKey == null) return;

            if (logEvent.Properties[entryKey] is StructureValue nested)
            {
                foreach (var prop in nested.Properties)
                {
                    logEvent.AddOrUpdateProperty(new LogEventProperty(prop.Name, prop.Value));
                }

                logEvent.RemovePropertyIfPresent(entryKey);
            }
        }
    }
}
