using AccountManagement.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Batch
{
    public class OrderProcessingBatchJob : BackgroundService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderProcessingBatchJob> _logger;

        public OrderProcessingBatchJob(IOrderService orderService, ILogger<OrderProcessingBatchJob> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting OrderProcessingBatchJob...");

            var orders = await _orderService.GetAllAsync(ct);
            foreach (var order in orders.Where(o => o.Status == "Pending"))
            {
                _logger.LogInformation("Processing Order {OrderId}", order.Id);

                // Example: mark as processed
                await _orderService.UpdateAsync(order, ct);
            }

            _logger.LogInformation("OrderProcessingBatchJob completed.");
            Environment.Exit(0); // one-shot execution
        }
    }

}
