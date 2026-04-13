using Serilog.Events;
using Serilog.Formatting;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public class OrderedFlatJsonFormatter : ITextFormatter
    {
        // Centralize all tags that should not be logged as a "Message"
        private static readonly HashSet<string> SystemTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "{@BatchEntry}", "{@BatchExit}", "{@BatchError}",
            "{@ApiEntry}", "{@ApiExit}", "{@ApiError}",
            "{@CronEntry}", "{@CronExit}", "{@CronError}",
            "{@DefaultEntry}", "{@DefaultExit}", "{@DefaultError}"
        };

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

                    case "Message":
                        var template = logEvent.MessageTemplate.Text;
                        if (SystemTags.Contains(template))
                            continue;

                        var renderedMessage = logEvent.RenderMessage();
                        dict[field] = renderedMessage.Replace("\"", "");
                        break;

                    case "SourceContext":
                        continue;

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
                if (prop.Key == "SourceContext")
                    continue;

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
