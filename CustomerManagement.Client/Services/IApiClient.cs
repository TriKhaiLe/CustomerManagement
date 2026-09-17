using System.Net.Http;

namespace CustomerManagement.Client.Services
{
    /// <summary>
    /// Centralizes HTTP handling so feature services receive either values or ApiException.
    /// The raw HttpClient property is kept only for callers that need to inspect the transport response directly.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Escape hatch for callers that must inspect the raw response; for example, login checks the HTTP status before
        /// deciding whether the credentials were invalid.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Performs a GET and returns the deserialized payload. Throws ApiException for any unsuccessful response.
        /// </summary>
        Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken ct = default);

        /// <summary>
        /// Performs a GET and returns the deserialized payload or default when the server returns 404.
        /// </summary>
        Task<TResponse?> GetOrDefaultAsync<TResponse>(string requestUri, CancellationToken ct = default);

        /// <summary>
        /// Performs a POST and returns the deserialized payload. Throws ApiException for any unsuccessful response.
        /// </summary>
        Task<TResponse> PostAsync<TResponse>(string requestUri, object payload, CancellationToken ct = default);

        /// <summary>
        /// Performs a PUT request. Throws ApiException for any unsuccessful response.
        /// </summary>
        Task PutAsync(string requestUri, object payload, CancellationToken ct = default);

        /// <summary>
        /// Performs a DELETE request. Throws ApiException for any unsuccessful response.
        /// </summary>
        Task DeleteAsync(string requestUri, CancellationToken ct = default);
    }
}
