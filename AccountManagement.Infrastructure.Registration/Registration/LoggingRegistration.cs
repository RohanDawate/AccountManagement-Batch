using AccountManagement.Application;
using AccountManagement.Infrastructure.Persistence;
using AccountManagement.Infrastructure.Registration.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AccountManagement.Infrastructure.Registration
{
    public static class LoggingRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            // Register repos and services
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            services.AddTransient<ApiLoggingInterceptor>();
            services.AddTransient<BatchLoggingInterceptor>();
            services.AddTransient<CronLoggingInterceptor>();
            services.AddTransient<DefaultLoggingInterceptor>();
        }

        public static void ConfigureLogging(LoggerConfiguration loggerConfig, LoggingConfig config)
        {
            loggerConfig
                .MinimumLevel.Information()
                .Enrich.With<FlatStructureEnricher>();

            // Apply encryption if enabled
            if (config.Options?.Enabled == true && config.Options.EncryptNonWhitelisted
                && !string.IsNullOrWhiteSpace(config.EncryptionKey)
                && !string.IsNullOrWhiteSpace(config.EncryptionIV))
            {
                var key = Convert.FromBase64String(config.EncryptionKey);
                var iv = Convert.FromBase64String(config.EncryptionIV);
                loggerConfig.Enrich.With(new EncryptionEnricher(config.Options, key, iv)); 
            }

            // Default to current directory if not provided
            var basePath = config.BasePath ?? Path.Combine(AppContext.BaseDirectory, "Logs");

            if (config.ContextType == "API")
            {
                string path = Path.Combine(basePath, "API", "accmgmt", "accmgmt-.json");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                loggerConfig
                    .WriteTo.Async(c => c.Console(new OrderedFlatJsonFormatter()))
                    .WriteTo.Async(f => f.File(
                        formatter: new OrderedFlatJsonFormatter(),
                        path: path,
                        rollingInterval: RollingInterval.Day));
            }
            else
            {
                string date = DateTimeOffset.Now.ToString("yyyyMMdd");
                string time = DateTimeOffset.Now.ToString("HHmm");
                string path = Path.Combine(basePath, config.ContextType, config.JobName, $"{config.JobName}_{date}_{time}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                loggerConfig
                    .WriteTo.Console(new OrderedFlatJsonFormatter())
                    .WriteTo.File(
                        formatter: new OrderedFlatJsonFormatter(),
                        path: path);
            }
        }
    }
}
