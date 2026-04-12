using AccountManagement.Domain.Entities;

namespace AccountManagement.Application
{
    public interface ICustomerService
    {
        public Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
        public Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);
        public Task<int> CreateAsync(Customer customer, CancellationToken ct = default);
        public Task UpdateAsync(Customer customer, CancellationToken ct = default);
        public Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
