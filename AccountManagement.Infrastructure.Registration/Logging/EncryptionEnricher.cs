using Serilog.Core;
using Serilog.Events;
using System.Security.Cryptography;
using System.Text;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public class EncryptionEnricher : ILogEventEnricher
    {
        private readonly LoggingOptions _options;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionEnricher(LoggingOptions options, byte[] key, byte[] iv)
        {
            _options = options;
            _key = key;
            _iv = iv;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            return;

            //if (!_options.EncryptNonWhitelisted) return;

            //foreach (var prop in logEvent.Properties)
            //{
            //    if (_options.WhitelistIdentifiers.Contains(prop.Key))
            //        continue;

            //    var value = prop.Value.ToString();
            //    var encrypted = Encrypt(value);
            //    var encoded = Convert.ToBase64String(encrypted); // compact + JSON safe

            //    logEvent.AddOrUpdateProperty(new LogEventProperty(prop.Key, new ScalarValue(encoded)));
            //}
        }

        private byte[] Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }
    }
}
