using Autofac;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace AccountManagement.Batch
{
    public class JobsModule : Autofac.Module
    {
        private readonly string _jobName;

        public JobsModule(string jobName)
        {
            _jobName = jobName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "AccountManagement.Batch";

            // Only scan assemblies that match your naming convention
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var jobType = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IBatchJob).IsAssignableFrom(t) && !t.IsAbstract)
                .FirstOrDefault(t => t.Name.Equals(_jobName, StringComparison.OrdinalIgnoreCase));

            if (jobType == null)
            {
                throw new InvalidOperationException($"Job {_jobName} not found in {assemblyName} assemblies.");
            }

            builder.RegisterType(jobType)
                   .As<IHostedService>()
                   .SingleInstance();
        }
    }

}
