using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public class FlatJsonFormatter : ITextFormatter
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var dict = new Dictionary<string, object?>
            {
                ["Timestamp"] = logEvent.Timestamp.ToString("o"),
                ["Level"] = logEvent.Level.ToString(),
                ["MessageTemplate"] = logEvent.MessageTemplate.Text,
            };

            // Flatten properties directly into root
            foreach (var prop in logEvent.Properties)
                dict[prop.Key] = Simplify(prop.Value);

            var json = JsonSerializer.Serialize(dict, _options);
            output.WriteLine(json);
        }

        private object? Simplify(LogEventPropertyValue value)
        {
            return value switch
            {
                ScalarValue s => s.Value, // preserves numbers, strings, etc.
                SequenceValue seq => seq.Elements.Select(Simplify).ToArray(),
                StructureValue str => str.Properties
                    .ToDictionary(p => p.Name, p => Simplify(p.Value)), // includes _typeTag
                DictionaryValue dict => dict.Elements.ToDictionary(
                    kvp => kvp.Key.Value?.ToString() ?? "",
                    kvp => Simplify(kvp.Value)),
                _ => value.ToString()

            };
        }
    }
}
