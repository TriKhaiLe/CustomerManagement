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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
