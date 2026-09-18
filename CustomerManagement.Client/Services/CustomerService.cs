using CustomerManagement.Shared;
using CustomerManagement.Shared.DTOs;

namespace CustomerManagement.Client.Services
{
    public class CustomerService : ICustomerService
    {
        private const string BasePath = "api/customers";

        private readonly IApiClient _apiClient;

        public CustomerService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQuery query, CancellationToken ct = default)
            => _apiClient.GetAsync<PagedResult<CustomerDto>>($"{BasePath}?{BuildQueryString(query)}", ct);

        public Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => _apiClient.GetOrDefaultAsync<CustomerDto>($"{BasePath}/{id}", ct);

        public Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
            => _apiClient.PostAsync<CustomerDto>(BasePath, request, ct);

        public Task UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default)
            => _apiClient.PutAsync($"{BasePath}/{id}", request, ct);

        public Task DeleteAsync(int id, CancellationToken ct = default)
            => _apiClient.DeleteAsync($"{BasePath}/{id}", ct);

        private static string BuildQueryString(CustomerQuery query)
        {
            var parts = new List<string>
            {
                $"pageNumber={query.PageNumber}",
                $"pageSize={query.PageSize}"
            };

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                parts.Add($"searchTerm={Uri.EscapeDataString(query.SearchTerm)}");

                if (query.SearchField is not null)
                {
                    parts.Add($"searchField={Uri.EscapeDataString(query.SearchField.Value.ToString())}");
                }
            }

            return string.Join("&", parts);
        }
    }
}
