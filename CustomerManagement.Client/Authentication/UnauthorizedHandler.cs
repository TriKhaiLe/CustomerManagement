using Microsoft.AspNetCore.Components;

namespace CustomerManagement.Client.Authentication
{
    public class UnauthorizedHandler : DelegatingHandler
    {
        private readonly JwtAuthenticationStateProvider _stateProvider;
        private readonly NavigationManager _navigationManager;

        public UnauthorizedHandler(JwtAuthenticationStateProvider stateProvider, NavigationManager navigationManager)
        {
            _stateProvider = stateProvider;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !request.IsOptedOut(AuthHttpRequestOptions.SkipUnauthorizedHandling))
            {
                await _stateProvider.NotifyUserLogoutAsync();

                var returnUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);

                if (returnUrl.StartsWith("login", StringComparison.OrdinalIgnoreCase))
                {
                    return response;
                }

                _navigationManager.NavigateTo(LoginRoute.WithReturnUrl(returnUrl));
            }

            return response;
        }
    }
}
