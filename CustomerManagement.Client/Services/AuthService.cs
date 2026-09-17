using System.Net;
using System.Net.Http.Json;
using CustomerManagement.Client.Models;

namespace CustomerManagement.Client.Services
{
    public class AuthService : IAuthService
    {
        private const string LoginEndpoint = "api/auth/login";
        private readonly IApiClient _apiClient;

        public AuthService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _apiClient.HttpClient.PostAsJsonAsync(LoginEndpoint, request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return LoginResult.Failure("Invalid username or password.");
            }

            if (!response.IsSuccessStatusCode)
            {
                return LoginResult.Failure($"Login failed with status {(int)response.StatusCode}.");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

            if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
            {
                return LoginResult.Failure("Login response did not contain an access token.");
            }

            var session = new AuthSession
            {
                AccessToken = loginResponse.AccessToken,
                Username = loginResponse.Username,
                ExpiresAtUtc = loginResponse.ExpiresAtUtc
            };

            return LoginResult.Success(session);
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public Task<AuthSession?> GetSessionAsync()
        {
            return Task.FromResult<AuthSession?>(null);
        }
    }
}
