using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace AccountManagement.Infrastructure.Registration.Logging
{
    public class OrderedFlatJsonFormatter : ITextFormatter
    {
        private static readonly string[] OrderedFields = {
            "Timestamp", "Level", "ContextType", "RunId", "TraceId", "Direction", "JobName", 
            "Class", "Method", "Args", "Error", "Exception"
        };

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var dict = new Dictionary<string, object?>();

            // Ordered core fields
            foreach (var field in OrderedFields)
            {
                switch (field)
                {
                    case "Timestamp": 
                        dict[field] = logEvent.Timestamp.ToString("o"); 
                        break;

                    case "Level": 
                        dict[field] = logEvent.Level.ToString(); 
                        break;

                    default:
                        if (logEvent.Properties.TryGetValue(field, out var value))
                            dict[field] = Simplify(value);
                        break;
                }
            }

            // Flatten any additional properties not in OrderedFields
            foreach (var prop in logEvent.Properties)
            {
                if (!dict.ContainsKey(prop.Key))
                    dict[prop.Key] = Simplify(prop.Value);
            }

            var json = JsonSerializer.Serialize(dict, _options);
            output.WriteLine(json);
        }

        private object? Simplify(LogEventPropertyValue value) =>
            value switch
            {
                ScalarValue s => s.Value,
                SequenceValue seq => seq.Elements.Select(Simplify).ToArray(),
                StructureValue str => str.Properties.ToDictionary(p => p.Name, p => Simplify(p.Value)),
                DictionaryValue dict => dict.Elements.ToDictionary(
                    kvp => kvp.Key.Value?.ToString() ?? "",
                    kvp => Simplify(kvp.Value)),
                _ => value.ToString()
            };
    }
}
