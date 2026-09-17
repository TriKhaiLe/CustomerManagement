namespace CustomerManagement.Client.Models
{
    public sealed record LoginResult(bool Succeeded, AuthSession? Session, string? Error)
    {
        public static LoginResult Success(AuthSession session) => new(true, session, null);

        public static LoginResult Failure(string error) => new(false, null, error);
    }
}
