using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class NotInTheFutureAttribute : ValidationAttribute
{
    public NotInTheFutureAttribute()
        : base("The date cannot be in the future.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value switch
        {
            DateTime dateTime => dateTime.Date <= DateTime.Today,
            DateTimeOffset dateTimeOffset => dateTimeOffset.Date <= DateTime.Today,
            DateOnly dateOnly => dateOnly <= DateOnly.FromDateTime(DateTime.Today),
            _ => false
        };
    }
}
