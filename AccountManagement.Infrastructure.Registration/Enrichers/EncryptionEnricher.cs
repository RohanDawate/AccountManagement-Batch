using Serilog.Core;
using Serilog.Events;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class EncryptionEnricher : ILogEventEnricher
    {
        private readonly LoggingOptions _options;
        private readonly LogEncryptionService _encryptionService;

        public EncryptionEnricher(LoggingOptions options, LogEncryptionService encryptionService)
        {
            _options = options;
            _encryptionService = encryptionService;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!_options.EncryptNonWhitelisted) return;

            // Use a ToList to avoid "Collection modified" errors if you were removing props
            var propertyKeys = logEvent.Properties.Keys.ToList();

            foreach (var key in propertyKeys)
            {
                // Skip Whitelisted keys (Timestamp, Level, etc.)
                if (_options.WhitelistIdentifiers.Contains(key))
                    continue;

                var propValue = logEvent.Properties[key];

                // We only want to encrypt the actual value, not the Serilog formatting quotes
                string rawValue = propValue is ScalarValue scalar ? scalar.Value?.ToString() : propValue.ToString();

                if (!string.IsNullOrEmpty(rawValue))
                {
                    var encrypted = _encryptionService.Encrypt(rawValue);
                    logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(key, encrypted));
                }
            }
        }
    }
}
