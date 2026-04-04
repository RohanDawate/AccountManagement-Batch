using AccountManagement.Domain.Entities;

namespace AccountManagement.Application
{
    public interface IOrderService
    {
        public Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct = default);
        public Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default);
        public Task<int> AddOrderAsync(Order order, CancellationToken ct = default);
        public Task UpdateOrderAsync(Order order, CancellationToken ct = default);
        public Task DeleteOrderAsync(int id, CancellationToken ct = default);
    }
}
