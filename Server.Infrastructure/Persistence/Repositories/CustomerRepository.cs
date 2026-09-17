using CustomerManagement.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces;
using Server.Domain.Common;
using Server.Domain.Entities;

namespace Server.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> CustomerCodeExistsAsync(string customerCode, CancellationToken cancellationToken = default) =>
        _context.Customers.AnyAsync(c => c.CustomerCode == customerCode, cancellationToken);

    public async Task<int> GetNextCustomerCodeSequenceAsync(CancellationToken cancellationToken = default)
    {
        var candidateCodes = await _context.Customers
            .Where(c => c.CustomerCode.StartsWith(CustomerCodeDefaults.AutoPrefix))
            .Select(c => c.CustomerCode)
            .ToListAsync(cancellationToken);

        var maxSequence = 0;
        foreach (var code in candidateCodes)
        {
            var suffix = code.Substring(CustomerCodeDefaults.AutoPrefix.Length);
            if (int.TryParse(suffix, out var value) && value > maxSequence)
            {
                maxSequence = value;
            }
        }

        return maxSequence + 1;
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Add(customer);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<Customer> Items, int TotalCount)> SearchAsync(
        CustomerSearchField? searchField,
        string? searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = searchField switch
            {
                CustomerSearchField.PhoneNumber => query.Where(c => EF.Functions.Like(c.PhoneNumber, pattern)),
                _ => query.Where(c => EF.Functions.Like(c.FullName, pattern))
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.FullName)
            .ThenBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
