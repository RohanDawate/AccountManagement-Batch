using AccountManagement.Application;
using AccountManagement.Domain.Entities;
using System.Collections.Concurrent;

namespace AccountManagement.Infrastructure.Persistence
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, Order> _orders = new();
        private int _nextId = 1;

        public OrderRepository()
        {
            // Seed sample orders
            AddSeedOrders();
        }

        private void AddSeedOrders()
        {
            CreateAsync(new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, Status = "Pending", TotalAmount = 250.00m }).Wait();
            CreateAsync(new Order { CustomerId = 2, OrderDate = DateTime.UtcNow, Status = "Processed", TotalAmount = 1200.50m }).Wait();
            CreateAsync(new Order { CustomerId = 3, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 75.00m }).Wait();
            CreateAsync(new Order { CustomerId = 4, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 560.00m }).Wait();
            CreateAsync(new Order { CustomerId = 5, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 310.25m }).Wait();
        }

        public Task<int> CreateAsync(Order order, CancellationToken ct = default)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var id = Interlocked.Increment(ref _nextId);
            order.Id = id;
            _orders[id] = order;

            return Task.FromResult(id);
        }

        public Task DeleteAsync(int id, CancellationToken ct = default)
        {
            _orders.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
        }

        public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }

        public Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (!_orders.ContainsKey(order.Id))
                throw new ArgumentException($"Order with id {order.Id} not found.");

            _orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
