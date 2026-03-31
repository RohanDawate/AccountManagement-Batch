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

        public async Task<int> CreateAsync(Order order, CancellationToken ct)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.TotalAmount <= 0)
                throw new ArgumentException("Order total must be greater than zero.");

            return await _repository.CreateAsync(order, ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing == null)
                throw new ArgumentException($"Order with id {id} not found.");

            await _repository.DeleteAsync(id, ct);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct)
        {
            return await _repository.GetAllAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _repository.GetByIdAsync(id, ct)
                    ?? throw new ArgumentException($"Order with id {id} not found.");
        }

        public async Task UpdateAsync(Order order, CancellationToken ct)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var existing = await _repository.GetByIdAsync(order.Id, ct);
            if (existing == null)
                throw new ArgumentException($"Order with id {order.Id} not found.");

            await _repository.UpdateAsync(order, ct);
        }
    }
}
