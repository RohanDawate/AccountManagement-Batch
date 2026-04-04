using AccountManagement.Infrastructure.Registration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
                            context.Configuration.GetSection("LoggingOptions"));
                    })
                    .UseSerilog((context, services, loggerConfig) =>
                    {
                        // Resolve IOptions<LoggingOptions> and then take the Value
                        var options = services.GetRequiredService<IOptions<LoggingOptions>>().Value;

                        var keyString = context.Configuration["Logging:EncryptionKey"];
                        var ivString = context.Configuration["Logging:EncryptionIV"];
                        var basePath = context.Configuration["Logging:BasePath"];

                        if (string.IsNullOrWhiteSpace(keyString) || string.IsNullOrWhiteSpace(ivString))
                        {
                            throw new InvalidOperationException("EncryptionKey or EncryptionIV is missing in configuration.");
                        }

                        var key = Convert.FromBase64String(keyString);
                        var iv = Convert.FromBase64String(ivString);

                        // Pass the raw LoggingOptions object into ConfigureLogging
                        LoggingRegistration.ConfigureLogging(loggerConfig, "Batch", jobName, options, key, iv, basePath);
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
