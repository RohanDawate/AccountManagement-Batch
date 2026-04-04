using AccountManagement.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AccountManagement.Batch.Jobs
{
    public class OrderGetJob : BackgroundService, IBatchJob
    {
        private readonly ILogger<OrderGetJob> _logger;
        private readonly IOrderService _orderService;

        public string Name => nameof(OrderGetJob);

        public OrderGetJob(IOrderService orderService, ILogger<OrderGetJob> logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting OrderGetJob...");

            var orders = await _orderService.GetAllOrdersAsync(ct);
            foreach (var order in orders.Where(o => o.Status == "Shipped"))
            {
                _logger.LogInformation("Processing Order {OrderId}", order.Id);
               
            }

            _logger.LogInformation("OrderGetJob completed.");
            Environment.Exit(0); // one-shot execution
        }
    }
}
