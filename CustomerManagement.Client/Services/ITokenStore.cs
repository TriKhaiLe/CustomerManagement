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

        // Initialize a session in-memory without writing to JS localStorage.
        // Useful for testing scenarios where the app should run with a fake session.
        Task InitializeInMemorySessionAsync(AuthSession session);

        Task ClearAsync();
    }
}
