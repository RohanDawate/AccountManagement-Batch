using AccountManagement.Domain.Entities;

namespace AccountManagement.Application
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);
        Task<int> CreateAsync(Customer customer, CancellationToken ct = default);
        Task UpdateAsync(Customer customer, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
