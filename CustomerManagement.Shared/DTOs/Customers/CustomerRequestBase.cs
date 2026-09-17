using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public abstract class CustomerRequestBase
{
    [Required(ErrorMessageResourceName = "FullNameRequired", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    [StringLength(200, ErrorMessageResourceName = "FullNameMaxLength", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    [StringLength(256, ErrorMessageResourceName = "EmailMaxLength", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^\+?\d[\d\s\-().]{6,19}$",
        ErrorMessageResourceName = "PhoneInvalid", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    public string? PhoneNumber { get; set; }

    // Use DateTime? instead of DateOnly? so it binds directly to MudDatePicker.
    [NotInTheFuture(ErrorMessageResourceName = "DateOfBirthFuture", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;
}
