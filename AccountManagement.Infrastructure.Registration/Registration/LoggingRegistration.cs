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

            services.AddTransient<ApiLoggingInterceptor>();
            services.AddTransient<BatchLoggingInterceptor>();
            services.AddTransient<CronLoggingInterceptor>();
            services.AddTransient<DefaultLoggingInterceptor>();
        }

        public static void ConfigureLogging(LoggerConfiguration loggerConfig, string contextType, string jobName = "default", 
            LoggingOptions? options = null, byte[]? key = null, byte[]? iv = null, string? basePath = null)
        {
            loggerConfig
                .MinimumLevel.Information()
                .Enrich.With<FlatStructureEnricher>();

            // Apply encryption if enabled
            if (options?.Enabled == true && options.EncryptNonWhitelisted && key != null && iv != null)
            {
                loggerConfig.Enrich.With(new EncryptionEnricher(options, key, iv));
            }

            // Default to current directory if not provided
            basePath ??= Path.Combine(AppContext.BaseDirectory, "Logs");

            if (contextType == "API")
            {
                string path = Path.Combine(basePath, "API", "accmgmt", "accmgmt-.json");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                loggerConfig
                    .WriteTo.Async(c => c.Console(new FlatJsonFormatter()))
                    .WriteTo.Async(f => f.File(
                        formatter: new FlatJsonFormatter(),
                        path: path,
                        rollingInterval: RollingInterval.Day));
            }
            else
            {
                string date = DateTimeOffset.Now.ToString("yyyyMMdd");
                string time = DateTimeOffset.Now.ToString("HHmm");
                string path = Path.Combine(basePath, contextType, jobName, $"{jobName}_{date}_{time}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                //loggerConfig
                //    .WriteTo.Async(c => c.Console(new FlatJsonFormatter()))
                //    .WriteTo.Async(f => f.File(
                //        formatter: new FlatJsonFormatter(),
                //        path: path));

                loggerConfig
                    .WriteTo.Console(new FlatJsonFormatter())
                    .WriteTo.File(
                        formatter: new FlatJsonFormatter(),
                        path: path);
            }
        }
    }
}
