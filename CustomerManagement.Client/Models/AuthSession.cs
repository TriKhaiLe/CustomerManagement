using System.Text.Json.Serialization;

namespace CustomerManagement.Client.Models
{
    public class AuthSession
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAtUtc { get; set; }

        [JsonIgnore]
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAtUtc;
    }
}
