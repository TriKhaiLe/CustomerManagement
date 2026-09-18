using System.Net;
using CustomerManagement.Shared.Common;

namespace CustomerManagement.Client.Services
{
    /// <summary>
    /// A single client-side exception type that carries the HTTP status, parsed error payload, and a safe message.
    /// The Message is always safe to show to a user.
    /// </summary>
    public class ApiException : Exception
    {
        private static readonly IReadOnlyDictionary<string, string[]> EmptyValidationErrors =
            new Dictionary<string, string[]>();

        public ApiException(
            HttpStatusCode? statusCode,
            string message,
            ApiErrorResponse? error = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public HttpStatusCode? StatusCode { get; }

        public ApiErrorResponse? Error { get; }

        public IReadOnlyDictionary<string, string[]> ValidationErrors => Error?.Errors ?? EmptyValidationErrors;

        public bool IsValidationProblem => ValidationErrors.Count > 0;

        public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

        public bool IsConflict => StatusCode == HttpStatusCode.Conflict;

        public bool IsServerError => StatusCode is not null && StatusCode.Value >= HttpStatusCode.InternalServerError;

        public bool IsNetworkError => StatusCode is null;
    }
}
