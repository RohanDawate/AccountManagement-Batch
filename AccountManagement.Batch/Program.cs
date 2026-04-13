using AccountManagement.Infrastructure.Registration;
using AccountManagement.Infrastructure.Registration.Enrichers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;

namespace AccountManagement.Batch
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var jobNameArg = args.FirstOrDefault(a => a.StartsWith("--job="));
            var jobName = jobNameArg?.Split('=')[1] ?? "OrderProcessingBatchJob";

            // Start timer at the very beginning of the logic
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Initializing the logs for context type and run id
                RunContext.Initialize("Batch");

                using (LogContext.PushProperty("JobName", jobName))
                {
                    var hostBuilder = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
                    {
                        containerBuilder.RegisterModule(new InfrastructureModule("Batch", jobName));
                        containerBuilder.RegisterModule(new JobsModule(jobName));
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // Bind LoggingOptions from appsettings.json
                        services.Configure<LoggingOptions>(
                            context.Configuration.GetSection("LoggingConfig:Options"));
                    })
                    .UseSerilog((context, services, loggerConfig) =>
                    {
                        // Bind strongly typed LoggingConfig from appsettings.json
                        var loggingConfig = context.Configuration.GetSection("LoggingConfig")
                                                                 .Get<LoggingConfig>();

                        if (loggingConfig == null)
                        {
                            throw new InvalidOperationException("LoggingConfig section is missing in configuration.");
                        }

                        var encryptionService = new LogEncryptionService(
                            loggingConfig.EncryptionKey ?? string.Empty,
                            loggingConfig.EncryptionIV ?? string.Empty
                        );

                        // Configure Serilog using the composite object
                        LoggingRegistration.ConfigureLogging(loggerConfig, loggingConfig, encryptionService);
                    });

                    var host = hostBuilder.Build();
                    var job = host.Services.GetRequiredService<IHostedService>();

                    Log.Information("Batch Process started for {JobName}.", jobName);

                    await job.StartAsync(CancellationToken.None);
                    await job.StopAsync(CancellationToken.None);
                }

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                watch.Stop();
                Log.Information("Batch Process finished for {JobName}. Total Execution Time: {DurationMs}ms", jobName, watch.ElapsedMilliseconds);
                Log.CloseAndFlush();
            }
        }

    }
}
