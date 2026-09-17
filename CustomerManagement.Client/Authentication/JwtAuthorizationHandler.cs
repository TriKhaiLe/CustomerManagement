using System.Net.Http.Headers;
using CustomerManagement.Client.Services;

namespace CustomerManagement.Client.Authentication
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly ITokenStore _tokenStore;

        public JwtAuthorizationHandler(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization is null && !request.IsOptedOut(AuthHttpRequestOptions.SkipBearerToken))
            {
                var accessToken = await _tokenStore.GetAccessTokenAsync();

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
