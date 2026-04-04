using AccountManagement.Application;
using AccountManagement.Domain.Entities;

namespace AccountManagement.Application
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<int> AddOrderAsync(Order order, CancellationToken ct)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.TotalAmount <= 0)
                throw new ArgumentException("Order total must be greater than zero.");

            return await _repository.AddOrderAsync(order, ct);
        }

        public async Task DeleteOrderAsync(int id, CancellationToken ct)
        {
            var existing = await _repository.GetOrderByIdAsync(id, ct);
            if (existing == null)
                throw new ArgumentException($"Order with id {id} not found.");

            await _repository.DeleteOrderAsync(id, ct);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken ct)
        {
            return await _repository.GetAllOrdersAsync(ct);
        }

        public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct)
        {
            return await _repository.GetOrderByIdAsync(id, ct)
                    ?? throw new ArgumentException($"Order with id {id} not found.");
        }

        public async Task UpdateOrderAsync(Order order, CancellationToken ct)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var existing = await _repository.GetOrderByIdAsync(order.Id, ct);
            if (existing == null)
                throw new ArgumentException($"Order with id {order.Id} not found.");

            await _repository.UpdateOrderAsync(order, ct);
        }
    }
}
