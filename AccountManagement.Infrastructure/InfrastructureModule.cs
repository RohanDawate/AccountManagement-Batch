using AccountManagement.Application;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace AccountManagement.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingInterceptor>().AsSelf().SingleInstance();

            builder.RegisterType<OrderRepository>()
                   .As<IOrderRepository>()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggingInterceptor));

            builder.RegisterType<OrderService>()
                   .As<IOrderService>()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggingInterceptor));

            builder.RegisterType<CustomerRepository>()
                   .As<ICustomerRepository>()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggingInterceptor));

            builder.RegisterType<CustomerService>()
                   .As<ICustomerService>()
                   .InstancePerLifetimeScope()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(LoggingInterceptor));

            // ... Supplier, Product, Order same pattern
        }
    }
}
