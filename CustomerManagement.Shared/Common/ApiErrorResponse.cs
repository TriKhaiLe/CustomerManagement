namespace CustomerManagement.Shared.Common;

public class ApiErrorResponse
{
    public int Status { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Field-level validation errors, keyed by field name. Null/empty for non-validation failures.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; init; }

    /// <summary>Correlation id for cross-referencing server logs.</summary>
    public string? TraceId { get; init; }
}
