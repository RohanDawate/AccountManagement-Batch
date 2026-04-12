namespace AccountManagement.Infrastructure.Registration
{
    public class LoggingOptions
    {
        public bool Enabled { get; set; } = true;
        public bool EnableArgumentLogging { get; set; } = true;
        public bool LogCancellationToken { get; set; } = true;
        public bool LogCancellationTokenDetails { get; set; } = true;
        public bool EncryptNonWhitelisted { get; set; } = true;
        public string[] WhitelistIdentifiers { get; set; } = Array.Empty<string>();
        public List<string> OrderedFields { get; set; } = new List<string>();
    }
}