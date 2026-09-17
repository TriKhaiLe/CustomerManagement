using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public abstract class CustomerRequestBase
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(200, ErrorMessage = "Full name cannot exceed 200 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^\+?\d[\d\s\-().]{6,19}$",
        ErrorMessage = "Enter a valid phone number, for example +61 2 5550 0100.")]
    public string? PhoneNumber { get; set; }

    // Use DateTime? instead of DateOnly? so it binds directly to MudDatePicker.
    [NotInTheFuture(ErrorMessage = "Date of birth cannot be in the future.")]
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;
}
