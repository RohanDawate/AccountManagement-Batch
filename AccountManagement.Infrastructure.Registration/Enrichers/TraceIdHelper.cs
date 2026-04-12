namespace AccountManagement.Infrastructure.Registration.Enrichers
{
    public static class TraceIdHelper
    {
        /// Generates a structured TraceId. If prefix is null, defaults to 'GEN'.
        public static string Generate(string? prefix = null)
        {
            var cleanPrefix = string.IsNullOrWhiteSpace(prefix)
                ? "GEN"
                : prefix.Trim().ToUpper();

            // Using 8 characters of a GUID for a balance between uniqueness and readability
            var uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            return $"{cleanPrefix}_{uniquePart}";
        }
    }
}
