using AccountManagement.Application;
using AccountManagement.Infrastructure.Persistence;
using AccountManagement.Infrastructure.Registration.Enrichers;
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

        public static void ConfigureLogging(LoggerConfiguration loggerConfig, LoggingConfig config, LogEncryptionService encryptionService)
        {
            // 1. Initialize the formatter with the ordered fields from appsettings
            var orderedFields = config.Options?.OrderedFields ?? new List<string>();
            var formatter = new OrderedFlatJsonFormatter(orderedFields);

            loggerConfig
                .MinimumLevel.Information()
                .Enrich.FromLogContext() // Keeps support for manual PushProperty
                .Enrich.With<GlobalContextEnricher>() // <--- Adds your TraceId and RunId automatically
                .Enrich.With<FlatStructureEnricher>();

            // Apply encryption if enabled
            if (config.Options?.Enabled == true && config.Options.EncryptNonWhitelisted)
            {
                var key = Convert.FromBase64String(config.EncryptionKey);
                var iv = Convert.FromBase64String(config.EncryptionIV);
                loggerConfig.Enrich.With(new EncryptionEnricher(config.Options, encryptionService)); 
            }

            // Default to current directory if not provided
            var basePath = config.BasePath ?? Path.Combine(AppContext.BaseDirectory, "Logs");

            if (config.ContextType == "API")
            {
                string path = Path.Combine(basePath, "API", "accmgmt", "accmgmt-.json");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                loggerConfig
                    .WriteTo.Async(c => c.Console(new OrderedFlatJsonFormatter(orderedFields)))
                    .WriteTo.Async(f => f.File(
                        formatter: new OrderedFlatJsonFormatter(orderedFields),
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
                    .WriteTo.Console(new OrderedFlatJsonFormatter(orderedFields))
                    .WriteTo.File(
                        formatter: new OrderedFlatJsonFormatter(orderedFields),
                        path: path);
            }
        }
    }
}
