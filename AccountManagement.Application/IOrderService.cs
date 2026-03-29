using AccountManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Application
{
    public interface IOrderService
    {
        public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
        public Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
        public Task<int> CreateAsync(Order order, CancellationToken ct = default);
        public Task UpdateAsync(Order order, CancellationToken ct = default);
        public Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
