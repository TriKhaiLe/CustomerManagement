using System.Text.Json;
using CustomerManagement.Client.Models;
using Microsoft.JSInterop;

namespace CustomerManagement.Client.Services
{
    public class TokenStore : ITokenStore
    {
        private const string StorageKey = "customermanagement.auth";

        private readonly IJSRuntime _jsRuntime;
        private AuthSession? _session;
        private bool _loaded;

        public TokenStore(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<AuthSession?> GetSessionAsync()
        {
            if (!_loaded)
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _session = null;
                    _loaded = true;
                    return null;
                }

                try
                {
                    _session = JsonSerializer.Deserialize<AuthSession>(json);
                }
                catch (JsonException)
                {
                    _session = null;
                }

                _loaded = true;
            }

            if (_session is not null && _session.IsExpired)
            {
                await ClearAsync();
                return null;
            }

            return _session;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var session = await GetSessionAsync();
            return session?.AccessToken;
        }

        public async Task SetSessionAsync(AuthSession session)
        {
            _session = session;
            var json = JsonSerializer.Serialize(session);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }

        public Task InitializeInMemorySessionAsync(AuthSession session)
        {
            _session = session;
            _loaded = true;
            return Task.CompletedTask;
        }

        public async Task ClearAsync()
        {
            _session = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
    }
}
