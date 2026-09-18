namespace CustomerManagement.Client.Models;

public class LoginResult
{
    public bool Succeeded { get; private set; }

    public AuthSession? Session { get; private set; }

    public string? Error { get; private set; }

    public static LoginResult Success(AuthSession session) => new() { Succeeded = true, Session = session };

    public static LoginResult Failure(string error) => new() { Error = error };
}
