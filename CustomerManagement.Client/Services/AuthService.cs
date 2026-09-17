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
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<CustomerManagement.Client.Resources.SharedResource> _localizer;

        public AuthService(IApiClient apiClient, ITokenStore tokenStore, JwtAuthenticationStateProvider stateProvider,
            Microsoft.Extensions.Localization.IStringLocalizer<CustomerManagement.Client.Resources.SharedResource> localizer)
        {
            _apiClient = apiClient;
            _tokenStore = tokenStore;
            _stateProvider = stateProvider;
            _localizer = localizer;
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
                return LoginResult.Failure(_localizer["Auth.InvalidCredentials"]);
            }

            if (!response.IsSuccessStatusCode)
            {
                return LoginResult.Failure(_localizer["Auth.LoginFailedWithStatus", (int)response.StatusCode]);
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

            if (loginResponse is null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
            {
                return LoginResult.Failure(_localizer["Auth.MissingAccessToken"]);
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
