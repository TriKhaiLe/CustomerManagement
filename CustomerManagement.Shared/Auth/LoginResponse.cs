namespace CustomerManagement.Shared.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }
}
