using Server.Domain.Entities;

namespace Server.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> CustomerCodeExistsAsync(string customerCode, CancellationToken cancellationToken = default);
    Task<int> GetNextCustomerCodeSequenceAsync(CancellationToken cancellationToken = default);
}
