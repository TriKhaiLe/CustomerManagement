using FluentValidation.Results;

namespace Server.Application.Common.Exceptions;

public class AppValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public AppValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());
        
        // Example:
        // failures:
        //   Name  -> "Name is required"
        //   Name  -> "Name must be at least 3 characters"
        //   Email -> "Email is invalid"
        //
        // Results:
        // {
        //   "Name":  ["Name is required", "Name must be at least 3 characters"],
        //   "Email": ["Email is invalid"]
        // }
    }
}
