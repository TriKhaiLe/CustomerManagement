using CustomerManagement.Shared.DTOs;

namespace Server.Application.Customers;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
