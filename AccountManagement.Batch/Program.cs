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
            var jobNameArg = args.FirstOrDefault(a => a.StartsWith("--job="));
            var jobName = jobNameArg?.Split('=')[1] ?? "OrderGetJob";

            LoggingRegistration.ConfigureLogging("Batch", jobName);

            try
            {
                Log.Information("Starting Batch Host for {JobName}...", jobName);

                var hostBuilder = Host.CreateDefaultBuilder(args)
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

                        // Register all jobs
                        services.AddHostedService<OrderProcessingBatchJob>();
                        services.AddHostedService<OrderGetJob>();

                        // Filter: only run the requested job
                        services.PostConfigure<HostOptions>(opts =>
                        {
                            opts.ServicesStartConcurrently = false;
                        });
                    })
                    .UseSerilog();

                var host = hostBuilder.Build();

                // Resolve only the requested job
                var job = host.Services.GetServices<IHostedService>()
                    .FirstOrDefault(s => s.GetType().Name == jobName);

                if (job == null)
                {
                    Log.Error("Job {JobName} not found.", jobName);
                    return;
                }

                await job.StartAsync(CancellationToken.None);
                await job.StopAsync(CancellationToken.None);
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
