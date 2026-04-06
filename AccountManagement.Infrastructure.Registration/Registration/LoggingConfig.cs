using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Infrastructure.Registration
{
    public class LoggingConfig
    {
        public string ContextType { get; set; } = string.Empty;
        public string JobName { get; set; } = "default";
        public LoggingOptions Options { get; set; } = new LoggingOptions();
        public string? BasePath { get; set; }
        public string? EncryptionKey { get; set; }
        public string? EncryptionIV { get; set; }
    }
}
