namespace CustomerManagement.Shared.DTOs;

public class CustomerDto
{
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
