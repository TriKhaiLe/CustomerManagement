using System.Globalization;
using Microsoft.JSInterop;

namespace CustomerManagement.Client.Services
{
    public class LanguageService : ILanguageService
    {
        private const string StorageKey = "customermanagement.language";
        private readonly IJSRuntime _js;
        private bool _initialized;

        private static readonly string[] Supported = { "vi-VN", "en-US" };

        public LanguageService(IJSRuntime js)
        {
            _js = js;
            CurrentCultureName = "vi-VN"; // default until initialized
        }

        public string CurrentCultureName { get; private set; }

        public async Task InitializeAsync()
        {
            if (_initialized)
            {
                return;
            }

            string? stored = null;
            try
            {
                stored = await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
            }
            catch
            {
                // localStorage may be unavailable; we'll fall back to defaults
            }

            var culture = IsSupported(stored) ? stored! : "vi-VN";
            ApplyCulture(culture);
            CurrentCultureName = culture;
            _initialized = true;
        }

        public async Task SetCultureAsync(string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName) || !IsSupported(cultureName))
            {
                return;
            }

            ApplyCulture(cultureName);
            CurrentCultureName = cultureName;

            try
            {
                await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, cultureName);
            }
            catch
            {
                // Ignore persistence failures; culture should still be applied in-memory
            }
        }

        private static bool IsSupported(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Array.IndexOf(Supported, name) >= 0;
        }

        private static void ApplyCulture(string cultureName)
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);

            // Set both DefaultThreadCurrentCulture and CurrentCulture so formatting and
            // resource lookups use the selected culture on the UI thread and any background
            // work that relies on the thread defaults.
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Also set the ambient culture on the current thread.
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
