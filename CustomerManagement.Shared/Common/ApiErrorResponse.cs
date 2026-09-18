namespace CustomerManagement.Shared.Common;

public class ApiErrorResponse
{
    public string? Type { get; set; }

    public string? Title { get; set; }

    public int? Status { get; set; }

    public string? Detail { get; set; }

    public string? Message { get; set; }

    public string? TraceId { get; set; }

    public Dictionary<string, string[]>? Errors { get; set; }

    public string? GetSummary()
    {
        return GetFirstNonBlank(Detail)
            ?? GetFirstNonBlank(Title)
            ?? GetFirstNonBlank(Message)
            ?? GetFirstNonBlank(Errors)
            ?? null;
    }

    private static string? GetFirstNonBlank(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static string? GetFirstNonBlank(Dictionary<string, string[]>? errors)
    {
        if (errors is null)
        {
            return null;
        }

        foreach (var value in errors.Values)
        {
            foreach (var message in value)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return message;
                }
            }
        }

        return null;
    }
}
