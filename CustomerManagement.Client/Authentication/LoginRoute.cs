namespace CustomerManagement.Client.Authentication
{
    public static class LoginRoute
    {
        public const string Path = "login";
        public const string ReturnUrlParameter = "returnUrl";

        public static string WithReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl.StartsWith("login", StringComparison.OrdinalIgnoreCase))
            {
                return Path;
            }

            return $"login?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }
    }
}
