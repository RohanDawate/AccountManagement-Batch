namespace AccountManagement.Infrastructure.Registration
{
    public class LoggingOptions
    {
        public bool EnableArgumentLogging { get; set; } = true;
        public bool LogCancellationToken { get; set; } = true;
        public bool LogCancellationTokenDetails { get; set; } = true;
    }
}
