namespace Server.Domain.Entities;

public class Customer
{
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
