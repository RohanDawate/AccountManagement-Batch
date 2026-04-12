using AccountManagement.Infrastructure.Registration;
using AccountManagement.Infrastructure.Registration.Enrichers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
            var jobName = jobNameArg?.Split('=')[1] ?? "OrderProcessingBatchJob";

            try
            {
                Log.Information("Starting Batch Host for {JobName}...", jobName);

                RunContext.Initialize("Batch");

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

                        var encryptionService = new LogEncryptionService(loggingConfig.EncryptionKey, loggingConfig.EncryptionIV);

                        // Configure Serilog using the composite object
                        LoggingRegistration.ConfigureLogging(loggerConfig, loggingConfig, encryptionService);
                    });

                var host = hostBuilder.Build();
                var job = host.Services.GetRequiredService<IHostedService>();

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
