using CustomerManagement.Client.Authentication;
using CustomerManagement.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
