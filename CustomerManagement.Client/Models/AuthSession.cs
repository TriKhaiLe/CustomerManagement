namespace CustomerManagement.Client.Models;

public class AuthSession
{
    public string AccessToken { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; set; }

    public bool IsExpired => ExpiresAtUtc <= DateTimeOffset.UtcNow;
}
