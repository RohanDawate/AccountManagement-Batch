using AccountManagement.Application;
using AccountManagement.Application.Interfaces.Data;
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
            AddSeedOrders();
        }

        private void AddSeedOrders()
        {
            AddOrderAsync(new Order { CustomerId = 1, OrderDate = DateTime.UtcNow, Status = "Pending", TotalAmount = 250.00m }).Wait();
            AddOrderAsync(new Order { CustomerId = 2, OrderDate = DateTime.UtcNow, Status = "Processed", TotalAmount = 1200.50m }).Wait();
            AddOrderAsync(new Order { CustomerId = 3, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 75.00m }).Wait();
            AddOrderAsync(new Order { CustomerId = 4, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 560.00m }).Wait();
            AddOrderAsync(new Order { CustomerId = 5, OrderDate = DateTime.UtcNow, Status = "Shipped", TotalAmount = 310.25m }).Wait();
        }

        // Explicit implementation of IRepository<Order>
        Task<Order?> IRepository<Order>.GetByIdAsync(int id, CancellationToken ct)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }

        Task<IEnumerable<Order>> IRepository<Order>.GetAllAsync(CancellationToken ct)
        {
            return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
        }

        Task<int> IRepository<Order>.AddAsync(Order order, CancellationToken ct)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var id = Interlocked.Increment(ref _nextId);
            order.Id = id;
            _orders[id] = order;

            return Task.FromResult(id);
        }

        Task IRepository<Order>.UpdateAsync(Order order, CancellationToken ct)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (!_orders.ContainsKey(order.Id))
                throw new ArgumentException($"Order with id {order.Id} not found.");

            _orders[order.Id] = order;
            return Task.CompletedTask;
        }

        Task IRepository<Order>.DeleteAsync(int id, CancellationToken ct)
        {
            _orders.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        // Expressive names visible on IOrderRepository
        public Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct = default)
            => ((IRepository<Order>)this).GetByIdAsync(id, ct);

        public Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default)
            => ((IRepository<Order>)this).GetAllAsync(ct);

        public Task<int> AddOrderAsync(Order order, CancellationToken ct = default)
            => ((IRepository<Order>)this).AddAsync(order, ct);

        public Task UpdateOrderAsync(Order order, CancellationToken ct = default)
            => ((IRepository<Order>)this).UpdateAsync(order, ct);

        public Task DeleteOrderAsync(int id, CancellationToken ct = default)
            => ((IRepository<Order>)this).DeleteAsync(id, ct);
    }
}
