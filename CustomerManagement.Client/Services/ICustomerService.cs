using CustomerManagement.Shared;
using CustomerManagement.Shared.DTOs;

namespace CustomerManagement.Client.Services
{
    /// <summary>
    /// Thin mapping over the customers API. Every method throws ApiException on failure, including when the server is unreachable,
    /// so callers need exactly one catch block.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Returns the paged customer list for the supplied query. Throws ApiException on failure, including when the server is unreachable.
        /// </summary>
        Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQuery query, CancellationToken ct = default);

        /// <summary>
        /// Returns the customer with the supplied id, or null when no customer has that id. Throws ApiException on failure,
        /// including when the server is unreachable.
        /// </summary>
        Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Creates the supplied customer and returns the created customer, including the id and code the API assigned.
        /// Throws ApiException on failure, including when the server is unreachable.
        /// </summary>
        Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);

        /// <summary>
        /// Updates the customer with the supplied id. Throws ApiException on failure, including when the server is unreachable.
        /// </summary>
        Task UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default);

        /// <summary>
        /// Deletes the customer with the supplied id. Throws ApiException on failure, including when the server is unreachable.
        /// </summary>
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
