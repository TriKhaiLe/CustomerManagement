using CustomerManagement.Shared.Common;
using CustomerManagement.Shared.DTOs;

namespace Server.Application.Customers;

public interface ICustomerService
{
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<CustomerDto>> SearchAsync(CustomerQueryParameters query, CancellationToken cancellationToken = default);

}
