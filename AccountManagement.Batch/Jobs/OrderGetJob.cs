using AccountManagement.Application;
using AccountManagement.Infrastructure.Registration.Enrichers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AccountManagement.Batch.Jobs
{
    public class OrderGetJob : BackgroundService, IBatchJob
    {
        private readonly ILogger<OrderGetJob> _logger;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IOrderService _orderService;

        public string Name => nameof(OrderGetJob);

        public OrderGetJob(IOrderService orderService, IHostApplicationLifetime lifetime, ILogger<OrderGetJob> logger)
        {
            _logger = logger;
            _lifetime = lifetime;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            // 1. Root Job TraceId (Parent)
            var mainTraceId = TraceIdHelper.Generate("JOB"); // Eventually use 5 digit job identifier

            using (TraceContext.BeginScope(mainTraceId))
            {
                try
                {
                    var orders = await _orderService.GetAllOrdersAsync(ct);
                    foreach (var order in orders.Where(o => o.Status == "Shipped"))
                    {
                        _logger.LogInformation("Processing Order {OrderId}", order.Id);

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Error during batch execution");
                }
                finally
                {
                    _lifetime.StopApplication();
                }
            }
        }
    }
}
