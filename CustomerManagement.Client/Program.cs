using CustomerManagement.Client.Authentication;
using CustomerManagement.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

namespace CustomerManagement.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                ?? throw new InvalidOperationException("ApiBaseUrl is not configured in appsettings.json.");

            // IHttpClientFactory resolves handlers from its own scope, so a scoped store would
            // hand the handlers a different instance than the component tree subscribes to.
            builder.Services.AddSingleton<ITokenStore, TokenStore>();
            builder.Services.AddSingleton<JwtAuthenticationStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<JwtAuthenticationStateProvider>());
            builder.Services.AddAuthorizationCore();
            builder.Services.AddTransient<JwtAuthorizationHandler>();
            builder.Services.AddTransient<UnauthorizedHandler>();

            builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddHttpMessageHandler<UnauthorizedHandler>()
            .AddHttpMessageHandler<JwtAuthorizationHandler>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            // Register the customer service implementation. When TestingMode is enabled
            // we'll replace the real API-backed service with an in-memory fake for UI testing.
            if (bool.TryParse(builder.Configuration["TestingMode"], out var testingMode) && testingMode)
            {
                builder.Services.AddScoped<ICustomerService, FakeCustomerService>();
            }
            else
            {
                builder.Services.AddScoped<ICustomerService, CustomerService>();
            }
            builder.Services.AddMudServices(config =>
            {
                // Bottom-right keeps toasts clear of the app bar and the table toolbar.
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

                // Deleting several rows in a row would otherwise stack identical toasts.
                config.SnackbarConfiguration.PreventDuplicates = true;

                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 4000;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            var host = builder.Build();

            // If the client is configured for testing mode, create a fake session
            // and initialize it in-memory so components and HTTP handlers can use it.
            if (bool.TryParse(builder.Configuration["TestingMode"], out var bypass) && bypass)
            {
                var tokenStore = host.Services.GetRequiredService<ITokenStore>();
                var stateProvider = host.Services.GetRequiredService<JwtAuthenticationStateProvider>();

                var fakeUsername = "tester";
                var header = "{\"alg\":\"none\",\"typ\":\"JWT\"}";
                var payload = System.Text.Json.JsonSerializer.Serialize(new { name = fakeUsername, role = new[] { "Admin" } });
                string ToBase64Url(string s)
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(s);
                    return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
                }

                var fakeToken = ToBase64Url(header) + "." + ToBase64Url(payload) + "." + "signature";

                var session = new CustomerManagement.Client.Models.AuthSession
                {
                    AccessToken = fakeToken,
                    Username = fakeUsername,
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await tokenStore.InitializeInMemorySessionAsync(session);
                await stateProvider.NotifyUserAuthentication(session);
            }

            await host.RunAsync();
        }
    }
}
