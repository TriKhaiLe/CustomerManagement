using CustomerManagement.Shared.Enums;
using Server.Domain.Entities;

namespace Server.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> CustomerCodeExistsAsync(string customerCode, CancellationToken cancellationToken = default);
    Task<int> GetNextCustomerCodeSequenceAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Customer> Items, int TotalCount)> SearchAsync(
        CustomerSearchField? searchField,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    void Update(Customer customer);
    void Remove(Customer customer);
}
