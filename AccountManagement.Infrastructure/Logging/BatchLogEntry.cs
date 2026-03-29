namespace AccountManagement.Infrastructure.Logging
{
    public class BatchLogEntry
    {
        public string Direction { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public object Args { get; set; } = new Dictionary<string, object>();
        public long DurationMs { get; set; } = 0; 
        public object? Result { get; set; }
        public string Exception { get; set; } = string.Empty;
    }
}
