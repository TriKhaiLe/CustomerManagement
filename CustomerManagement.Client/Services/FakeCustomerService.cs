using CustomerManagement.Shared;
using CustomerManagement.Shared.DTOs;

namespace CustomerManagement.Client.Services;

public class FakeCustomerService : ICustomerService
{
    private readonly List<CustomerDto> _store = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public FakeCustomerService()
    {
        Seed();
    }

    private void Seed()
    {
        var now = DateTimeOffset.UtcNow;

        // Seed a larger set of customers to exercise paging, searching and sorting in the UI.
        var firstNames = new[] { "Alice", "Bob", "Charlie", "Dana", "Eve", "Frank", "Grace", "Hank", "Ivy", "Jack" };
        var lastNames = new[] { "Johnson", "Smith", "Brown", "Taylor", "Anderson", "Lee", "Walker", "Hall", "Young", "King" };

        var rnd = new Random(12345);

        for (var i = 0; i < 200; i++)
        {
            var fn = firstNames[rnd.Next(firstNames.Length)];
            var ln = lastNames[rnd.Next(lastNames.Length)];
            var full = $"{fn} {ln}";
            var code = $"CUST{i + 1:D4}";
            var email = $"{fn.ToLowerInvariant()}.{ln.ToLowerInvariant()}{i % 10}@example.com";
            var created = now.AddDays(-rnd.Next(0, 365)).AddMinutes(-rnd.Next(0, 1440));
            var active = (i % 5) != 0; // roughly 20% inactive

            Add(new CustomerDto
            {
                CustomerCode = code,
                FullName = full,
                Email = email,
                IsActive = active,
                CreatedAt = created
            });
        }
    }

    private CustomerDto Add(CustomerDto c)
    {
        if (c is null) throw new ArgumentNullException(nameof(c));
        lock (_lock)
        {
            c.Id = _nextId++;
            _store.Add(c);
            return c;
        }
    }

    public Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQuery query, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var items = _store.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.Trim();
                items = items.Where(x => x.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)
                                         || x.Email.Contains(term, StringComparison.OrdinalIgnoreCase)
                                         || x.CustomerCode.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var total = items.Count();
            var pageSize = Math.Max(1, query.PageSize);
            var pageNumber = Math.Max(1, query.PageNumber);
            var paged = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var result = new PagedResult<CustomerDto>
            {
                Items = paged,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber * pageSize < total
            };

            return Task.FromResult(result);
        }
    }

    public Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var item = _store.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(item);
        }
    }

    public Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var dto = new CustomerDto
        {
            CustomerCode = request.CustomerCode!,
            FullName = request.FullName!,
            Email = request.Email!,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var added = Add(dto);
        return Task.FromResult(added);
    }

    public Task UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var existing = _store.FirstOrDefault(x => x.Id == id);
            if (existing is null) throw new KeyNotFoundException("Customer not found");

            existing.FullName = request.FullName!;
            existing.Email = request.Email!;
            existing.PhoneNumber = request.PhoneNumber;
            existing.DateOfBirth = request.DateOfBirth;
            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var existing = _store.FirstOrDefault(x => x.Id == id);
            if (existing is not null)
            {
                _store.Remove(existing);
            }

            return Task.CompletedTask;
        }
    }
}
