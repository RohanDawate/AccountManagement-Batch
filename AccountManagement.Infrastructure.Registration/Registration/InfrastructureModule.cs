using AccountManagement.Application;
using AccountManagement.Infrastructure.Persistence;
using AccountManagement.Infrastructure.Registration.Logging;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace AccountManagement.Infrastructure.Registration
{
    public class InfrastructureModule : Module
    {
        private readonly string _contextType;
        private readonly string _jobName;

        public InfrastructureModule(string contextType, string jobName = "default")
        {
            _contextType = contextType;
            _jobName = jobName;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 1. Register Interceptors first
            RegisterInterceptors(builder);

            // 2. Scan Assemblies for Repositories and Services
            var infrastructureAssembly = typeof(OrderRepository).Assembly;
            var applicationAssembly = typeof(IOrderService).Assembly;

            // Get the correct interceptor type based on _contextType (API, Batch, Cron)
            var interceptorType = GetInterceptorType();

            // Register Repositories (Infrastructure Assembly)
            builder.RegisterAssemblyTypes(infrastructureAssembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .InterceptedBy(interceptorType) 
                .InstancePerLifetimeScope();

            // Register Services (Application Assembly)
            builder.RegisterAssemblyTypes(applicationAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .InterceptedBy(interceptorType)
                .InstancePerLifetimeScope();
        }

        private void RegisterInterceptors(ContainerBuilder builder)
        {
            switch (_contextType)
            {
                case "API":
                    builder.RegisterType<ApiLoggingInterceptor>();
                    break;

                case "Batch":
                    builder.RegisterType<BatchLoggingInterceptor>()
                           .WithParameter("jobName", _jobName);
                    break;

                case "Cron":
                    builder.RegisterType<CronLoggingInterceptor>()
                           .WithParameter("jobName", _jobName);
                    break;

                default:
                    builder.RegisterType<DefaultLoggingInterceptor>();
                    break;
            }
        }

        private Type GetInterceptorType()
        {
            return _contextType switch
            {
                "API" => typeof(ApiLoggingInterceptor),
                "Batch" => typeof(BatchLoggingInterceptor),
                "Cron" => typeof(CronLoggingInterceptor),
                _ => typeof(DefaultLoggingInterceptor)
            };
        }

    }

}
