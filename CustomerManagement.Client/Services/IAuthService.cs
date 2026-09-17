using CustomerManagement.Client.Models;

namespace CustomerManagement.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        Task LogoutAsync();

        Task<AuthSession?> GetSessionAsync();
    }
}
