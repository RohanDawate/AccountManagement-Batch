using AccountManagement.Application;
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
            _logger.LogInformation("Starting OrderProcessingBatchJob...");

            var orders = await _orderService.GetAllOrdersAsync(ct);
            foreach (var order in orders.Where(o => o.Status == "Pending"))
            {
                _logger.LogInformation("Processing Order {OrderId}", order.Id);
                await _orderService.UpdateOrderAsync(order, ct);
            }

            _logger.LogInformation("OrderProcessingBatchJob completed.");
            Environment.Exit(0); // one-shot execution
        }
    }

}
