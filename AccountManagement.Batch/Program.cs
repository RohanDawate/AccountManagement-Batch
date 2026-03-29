using AccountManagement.Infrastructure;       // Fixes InfrastructureModule
using Autofac;
using Autofac.Extensions.DependencyInjection; // Fixes AutofacServiceProviderFactory
using Microsoft.Extensions.DependencyInjection; // Fixes .AddHostedService()
using Microsoft.Extensions.Hosting;           // Fixes IHostBuilder and IServiceCollection extensions
using Serilog;                                // Fixes .UseSerilog()


namespace AccountManagement.Batch
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog sinks
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .WriteTo.Console()
                 .WriteTo.File(
                     formatter: new Serilog.Formatting.Json.JsonFormatter(),
                     path: "Logs/batch-log-.json",
                     rollingInterval: RollingInterval.Day,
                     retainedFileCountLimit: 7, // keep last 7 days
                     fileSizeLimitBytes: 10_000_000, // 10 MB per file
                     rollOnFileSizeLimit: true)
                 .CreateLogger();


            try
            {
                var builder = Host.CreateDefaultBuilder(args)
                    //.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))
                    .UseSerilog()
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(cb =>
                    {
                        cb.RegisterModule(new InfrastructureModule());
                    })
                    .ConfigureServices((ctx, services) =>
                    {
                        services.AddHostedService<OrderProcessingBatchJob>();
                    });

                await builder.Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Batch application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
