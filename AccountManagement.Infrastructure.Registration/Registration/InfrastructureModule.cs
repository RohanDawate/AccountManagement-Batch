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

            // Repos & services
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();
            builder.RegisterType<OrderService>()
                   .As<IOrderService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(GetInterceptorType());

            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<CustomerService>()
                   .As<ICustomerService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(GetInterceptorType());

            // Register the chosen interceptor
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

//// Register interceptors
//builder.RegisterType<ApiLoggingInterceptor>();
//builder.RegisterType<BatchLoggingInterceptor>()
//       .WithParameter("jobName", _jobName);
//builder.RegisterType<CronLoggingInterceptor>()
//       .WithParameter("jobName", _jobName);
//builder.RegisterType<DefaultLoggingInterceptor>();
