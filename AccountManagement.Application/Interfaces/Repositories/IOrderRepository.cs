using AccountManagement.Application.Interfaces.Data;
using AccountManagement.Domain.Entities;

namespace AccountManagement.Application
{
    public interface IOrderRepository : IRepository<Order>
    {
        // Domain-specific expressive names
        Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<int> AddOrderAsync(Order order, CancellationToken ct = default);
        Task UpdateOrderAsync(Order order, CancellationToken ct = default);
        Task DeleteOrderAsync(int id, CancellationToken ct = default);
    }
}
