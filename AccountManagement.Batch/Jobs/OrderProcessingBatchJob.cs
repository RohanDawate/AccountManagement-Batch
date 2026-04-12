using AccountManagement.Application;
using AccountManagement.Domain.Entities;
using AccountManagement.Infrastructure.Registration.Enrichers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AccountManagement.Batch.Jobs
{
    public class OrderProcessingBatchJob : BackgroundService, IBatchJob
    {
        private readonly ILogger<OrderProcessingBatchJob> _logger;
        private readonly IOrderService _orderService;

        public string Name => nameof(OrderProcessingBatchJob);

        public OrderProcessingBatchJob(IOrderService orderService, ILogger<OrderProcessingBatchJob> logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            // 1. Root Job TraceId (Parent)
            var mainTraceId = TraceIdHelper.Generate("JOB"); // Eventually use 5 digit job identifier

            using (TraceContext.BeginScope(mainTraceId))
            {
                _logger.LogInformation("Starting OrderProcessingBatchJob...");

                try
                {
                    List<Order> orders;

                    // 2. Separate Scope for "Get All Orders"
                    using (TraceContext.BeginScope(TraceIdHelper.Generate("ORD_GET")))
                    {
                        _logger.LogInformation("Fetching pending orders from database...");
                        orders = (await _orderService.GetAllOrdersAsync(ct)).ToList();
                    }

                    foreach (var order in orders.Where(o => o.Status == "Pending"))
                    {
                        // 3. Separate Scope for "Each Individual Record Update"
                        using (TraceContext.BeginScope(TraceIdHelper.Generate("ORD_UPD")))
                        {
                            _logger.LogInformation("Processing Order {OrderId}", order.Id);
                            await _orderService.UpdateOrderAsync(order, ct);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during batch execution");
                }

                _logger.LogInformation("OrderProcessingBatchJob completed.");
            }

            Environment.Exit(0);
        }
    }

}
