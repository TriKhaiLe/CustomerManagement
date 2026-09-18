using System.Net.Http;

namespace CustomerManagement.Client.Authentication
{
    public static class AuthHttpRequestOptions
    {
        public static readonly HttpRequestOptionsKey<bool> SkipBearerToken = new("CustomerManagement.SkipBearerToken");
        public static readonly HttpRequestOptionsKey<bool> SkipUnauthorizedHandling = new("CustomerManagement.SkipUnauthorizedHandling");
    }

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage WithoutAuthentication(this HttpRequestMessage request)
        {
            request.Options.Set(AuthHttpRequestOptions.SkipBearerToken, true);
            request.Options.Set(AuthHttpRequestOptions.SkipUnauthorizedHandling, true);
            return request;
        }

        public static bool IsOptedOut(this HttpRequestMessage request, HttpRequestOptionsKey<bool> key)
        {
            return request.Options.TryGetValue(key, out var value) && value;
        }
    }
}
