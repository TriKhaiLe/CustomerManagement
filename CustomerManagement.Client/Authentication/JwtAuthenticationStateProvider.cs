using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CustomerManagement.Client.Models;
using CustomerManagement.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CustomerManagement.Client.Authentication
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

        private readonly ITokenStore _tokenStore;

        public JwtAuthenticationStateProvider(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var session = await _tokenStore.GetSessionAsync();

            if (session is null)
            {
                return Anonymous;
            }

            return BuildAuthenticationState(session);
        }

        public async Task NotifyUserAuthentication(AuthSession session)
        {
            var state = BuildAuthenticationState(session);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            await Task.CompletedTask;
        }

        public async Task NotifyUserLogoutAsync()
        {
            await _tokenStore.ClearAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
        }

        private static AuthenticationState BuildAuthenticationState(AuthSession session)
        {
            var claims = ParseJwtClaims(session.AccessToken);

            if (claims.Count == 0)
            {
                claims.Add(new Claim(ClaimTypes.Name, session.Username));
            }

            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private static List<Claim> ParseJwtClaims(string accessToken)
        {
            var tokenParts = accessToken.Split('.');

            if (tokenParts.Length != 3)
            {
                return [];
            }

            var payload = tokenParts[1];

            try
            {
                var normalized = payload.Replace('-', '+').Replace('_', '/');
                var padding = normalized.Length % 4;
                if (padding > 0)
                {
                    normalized += new string('=', 4 - padding);
                }

                var bytes = Convert.FromBase64String(normalized);
                var json = Encoding.UTF8.GetString(bytes);
                var claims = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (claims is null)
                {
                    return [];
                }

                var parsedClaims = new List<Claim>();

                foreach (var claim in claims)
                {
                    var claimName = MapClaimName(claim.Key);
                    var values = claim.Value.ValueKind switch
                    {
                        JsonValueKind.Array => claim.Value.EnumerateArray().Select(x => x.ToString()).ToArray(),
                        JsonValueKind.Null => [],
                        _ => [claim.Value.ToString()]
                    };

                    foreach (var value in values)
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            parsedClaims.Add(new Claim(claimName, value));
                        }
                    }
                }

                if (parsedClaims.All(x => x.Type != ClaimTypes.Name))
                {
                    return [];
                }

                return parsedClaims;
            }
            catch (JsonException)
            {
                return [];
            }
            catch (FormatException)
            {
                return [];
            }
        }

        private static string MapClaimName(string key)
        {
            return key.ToLowerInvariant() switch
            {
                "name" or "unique_name" or "preferred_username" => ClaimTypes.Name,
                "role" or "roles" => ClaimTypes.Role,
                "sub" => ClaimTypes.NameIdentifier,
                "email" => ClaimTypes.Email,
                _ => key
            };
        }
    }
}
