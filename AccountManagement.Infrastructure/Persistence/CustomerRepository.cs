using AccountManagement.Application;
using AccountManagement.Domain.Entities;

namespace AccountManagement.Infrastructure.Persistence
{
    public class CustomerRepository : ICustomerRepository
    {
        public Task<int> CreateAsync(Customer customer, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Customer customer, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
