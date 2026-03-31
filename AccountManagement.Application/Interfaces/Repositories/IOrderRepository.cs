using AccountManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagement.Application
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
        Task<int> CreateAsync(Order order, CancellationToken ct = default);
        Task UpdateAsync(Order order, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
