using CustomerManagement.Client.Services;
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

            builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
