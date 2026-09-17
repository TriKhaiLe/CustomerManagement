namespace CustomerManagement.Shared.DTOs;

public class CustomerDto
{
    public int Id { get; init; }

    public string CustomerCode { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string PhoneNumber { get; init; } = string.Empty;

    public DateOnly? DateOfBirth { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
