using Serilog.Events;
using Serilog.Formatting;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class OrderedFlatJsonFormatter : ITextFormatter
    {
        //private static readonly string[] OrderedFields = {
        //    "Timestamp", "Level", "ContextType", "RunId", "TraceId", "JobName", 
        //    "Direction", "Class", "Method", "DurationMs", "Args", "Error", "Exception"
        //};

        private readonly List<string> _orderedFields;

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Inject the fields here
        public OrderedFlatJsonFormatter(List<string> orderedFields)
        {
            // Fallback to a default list if config is empty
            _orderedFields = orderedFields ?? new List<string> { "Timestamp", "Level", "Message" };
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var dict = new Dictionary<string, object?>();

            // Ordered core fields
            foreach (var field in _orderedFields)
            {
                switch (field)
                {
                    case "Timestamp": 
                        dict[field] = logEvent.Timestamp.ToString("o"); 
                        break;

                    case "Level": 
                        dict[field] = logEvent.Level.ToString(); 
                        break;

                    case "Exception":
                        if (logEvent.Exception != null)
                            dict[field] = logEvent.Exception.ToString();
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
