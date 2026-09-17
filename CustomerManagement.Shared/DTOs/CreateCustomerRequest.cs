namespace CustomerManagement.Shared.DTOs;

public class CreateCustomerRequest
{
    public string? CustomerCode { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string PhoneNumber { get; init; } = string.Empty;

    public DateOnly? DateOfBirth { get; init; }

    public bool IsActive { get; init; } = true;
}
