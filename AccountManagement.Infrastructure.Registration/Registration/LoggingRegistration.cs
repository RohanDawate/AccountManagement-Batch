using AccountManagement.Application;
using AccountManagement.Infrastructure.Registration.Logging;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using AccountManagement.Infrastructure.Persistence;


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

        public static void ConfigureLogging(string contextType, string jobName = "default")
        {
            if (contextType == "API")
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(
                        formatter: new Serilog.Formatting.Json.JsonFormatter(),
                        path: "Logs/API/accmgmt/accmgmt-.json", // base name
                        rollingInterval: RollingInterval.Day) // Serilog appends yyyyMMdd
                    .CreateLogger();
            }
            else
            {
                string date = DateTimeOffset.Now.ToString("yyyyMMdd");
                string time = DateTimeOffset.Now.ToString("HHmm");

                string path = $"Logs/{contextType}/{jobName}/{jobName}_{date}_{time}.json";

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(
                        formatter: new Serilog.Formatting.Json.JsonFormatter(),
                        path: path)
                    .CreateLogger();
            }
        }
    }

}
