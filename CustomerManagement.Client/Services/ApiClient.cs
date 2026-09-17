namespace CustomerManagement.Client.Services
{
    public class ApiClient : IApiClient
    {
        public ApiClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }
    }
}
