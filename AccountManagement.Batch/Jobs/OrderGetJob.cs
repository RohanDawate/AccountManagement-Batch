using AccountManagement.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Batch.Jobs
{
    public class OrderGetJob : BackgroundService
    {
        private readonly ILogger<OrderProcessingBatchJob> _logger;
        private readonly IOrderService _orderService;

        public OrderGetJob(IOrderService orderService, ILogger<OrderProcessingBatchJob> logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting OrderGetJob...");

            var orders = await _orderService.GetAllAsync(ct);
            foreach (var order in orders.Where(o => o.Status == "Shipped"))
            {
                _logger.LogInformation("Processing Order {OrderId}", order.Id);
               
            }

            _logger.LogInformation("OrderGetJob completed.");
            Environment.Exit(0); // one-shot execution
        }
    }
}
