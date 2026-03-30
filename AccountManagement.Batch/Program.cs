using AccountManagement.Batch.Jobs;
using AccountManagement.Infrastructure.Registration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AccountManagement.Batch
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var jobName = typeof(OrderProcessingBatchJob).Name;
            LoggingRegistration.ConfigureLogging("Batch", jobName);

            try
            {
                Log.Information("Starting Batch Host...");

                // Use HostBuilder (not HostApplicationBuilder)
                var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                    {
                        // Register infrastructure services + interceptors
                        containerBuilder.RegisterModule(new InfrastructureModule("Batch", jobName));
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // Bind LoggingOptions from appsettings.json
                        services.Configure<LoggingOptions>(
                            context.Configuration.GetSection("LoggingOptions"));

                        // Register the batch job itself
                        services.AddHostedService<OrderProcessingBatchJob>();
                    })
                    .UseSerilog()
                    .Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
