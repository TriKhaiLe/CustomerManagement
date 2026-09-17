using CustomerManagement.Client.Models;

namespace CustomerManagement.Client.Services
{
    // Kept separate from IAuthService so the HTTP message handlers can read the token
    // without depending on IApiClient, which would create a dependency cycle.
    public interface ITokenStore
    {
        Task<AuthSession?> GetSessionAsync();

        Task<string?> GetAccessTokenAsync();

        Task SetSessionAsync(AuthSession session);

        Task ClearAsync();
    }
}
