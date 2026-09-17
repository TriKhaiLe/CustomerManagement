using System.Net;
using System.Net.Http.Json;
using CustomerManagement.Client.Authentication;
using CustomerManagement.Client.Models;
using CustomerManagement.Shared.Auth;

namespace CustomerManagement.Client.Services
{
    public class AuthService : IAuthService
    {
        private const string LoginEndpoint = "api/auth/login";
        private readonly IApiClient _apiClient;
        private readonly ITokenStore _tokenStore;
        private readonly JwtAuthenticationStateProvider _stateProvider;

        public AuthService(IApiClient apiClient, ITokenStore tokenStore, JwtAuthenticationStateProvider stateProvider)
        {
            _apiClient = apiClient;
            _tokenStore = tokenStore;
            _stateProvider = stateProvider;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, LoginEndpoint)
            {
                Content = JsonContent.Create(request)
            };

            message.WithoutAuthentication();

            var response = await _apiClient.HttpClient.SendAsync(message, cancellationToken);

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

            await _tokenStore.SetSessionAsync(session);
            await _stateProvider.NotifyUserAuthentication(session);

            return LoginResult.Success(session);
        }

        public Task LogoutAsync()
        {
            return _stateProvider.NotifyUserLogoutAsync();
        }

        public Task<AuthSession?> GetSessionAsync()
        {
            return _tokenStore.GetSessionAsync();
        }
    }
}
