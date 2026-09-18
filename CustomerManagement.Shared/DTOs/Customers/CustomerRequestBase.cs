using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public abstract class CustomerRequestBase
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150, ErrorMessage = "Full name must not exceed 150 characters.")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    [StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [StringLength(15, ErrorMessage = "Phone number must not exceed 15 characters.")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must contain numbers only.")]
    public string PhoneNumber { get; set; } = string.Empty;

    // Use DateTime? instead of DateOnly? so it binds directly to MudDatePicker.
    [NotInTheFuture(ErrorMessage = "Date of birth cannot be in the future.")]
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;
}
