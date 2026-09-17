using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CustomerManagement.Shared.Common;

namespace CustomerManagement.Client.Services
{
    public class ApiClient : IApiClient
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public ApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken ct = default)
        {
            var response = await SendAsync(() => HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, ct), ct);
            await ThrowIfFailedAsync(response, ct);
            return await ReadAsync<TResponse>(response, ct);
        }

        public async Task<TResponse?> GetOrDefaultAsync<TResponse>(string requestUri, CancellationToken ct = default)
        {
            var response = await SendAsync(() => HttpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, ct), ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }

            await ThrowIfFailedAsync(response, ct);
            return await ReadAsync<TResponse>(response, ct);
        }

        public async Task<TResponse> PostAsync<TResponse>(string requestUri, object payload, CancellationToken ct = default)
        {
            var response = await SendAsync(() => HttpClient.PostAsync(requestUri, Serialize(payload), ct), ct);
            await ThrowIfFailedAsync(response, ct);
            return await ReadAsync<TResponse>(response, ct);
        }

        public async Task PutAsync(string requestUri, object payload, CancellationToken ct = default)
        {
            var response = await SendAsync(() => HttpClient.PutAsync(requestUri, Serialize(payload), ct), ct);
            await ThrowIfFailedAsync(response, ct);
        }

        public async Task DeleteAsync(string requestUri, CancellationToken ct = default)
        {
            var response = await SendAsync(() => HttpClient.DeleteAsync(requestUri, ct), ct);
            await ThrowIfFailedAsync(response, ct);
        }

        private static JsonContent Serialize(object payload)
        {
            // The runtime type matters here; the declared type is always object, and this preserves enum naming and polymorphic payloads.
            return JsonContent.Create(payload, payload.GetType(), options: SerializerOptions);
        }

        private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send, CancellationToken ct)
        {
            try
            {
                return await send();
            }
            catch (HttpRequestException e)
            {
                throw new ApiException(null, "Cannot reach the server. Check your connection and try again.", innerException: e);
            }
            catch (TaskCanceledException e) when (!ct.IsCancellationRequested)
            {
                throw new ApiException(null, "The request timed out. Please try again.", innerException: e);
            }
        }

        private static async Task ThrowIfFailedAsync(HttpResponseMessage response, CancellationToken ct)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error = await ReadErrorAsync(response, ct);
            var statusCode = response.StatusCode;
            throw new ApiException(statusCode, BuildMessage(statusCode, error), error);
        }

        private static async Task<ApiErrorResponse?> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
        {
            if (response.Content is null || response.Content.Headers.ContentLength == 0)
            {
                return null;
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<ApiErrorResponse>(json, SerializerOptions);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private static async Task<TResponse> ReadAsync<TResponse>(HttpResponseMessage response, CancellationToken ct)
        {
            try
            {
                var payload = await response.Content.ReadFromJsonAsync<TResponse>(SerializerOptions, ct);
                if (payload is null)
                {
                    throw new ApiException(response.StatusCode, "The server returned an empty response.");
                }

                return payload;
            }
            catch (JsonException)
            {
                throw new ApiException(response.StatusCode, "The server returned a response this app could not read.");
            }
            catch (NotSupportedException)
            {
                throw new ApiException(response.StatusCode, "The server returned a response this app could not read.");
            }
        }

        private static string BuildMessage(HttpStatusCode statusCode, ApiErrorResponse? error)
        {
            if (statusCode >= HttpStatusCode.InternalServerError)
            {
                return "The server had a problem completing that request. Please try again.";
            }

            var summary = error?.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary))
            {
                return summary;
            }

            return statusCode switch
            {
                HttpStatusCode.BadRequest => "The request was rejected as invalid.",
                HttpStatusCode.Unauthorized => "Your session has expired. Please sign in again.",
                HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
                HttpStatusCode.NotFound => "The requested item was not found.",
                HttpStatusCode.Conflict => "That change conflicts with the current data. Reload and try again.",
                _ => $"The server returned {(int)statusCode} ({statusCode})."
            };
        }
    }
}
