using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public class CreateCustomerRequest : CustomerRequestBase
{
    // This is optional; the server assigns one when blank, and it cannot be changed later.
    [StringLength(20, ErrorMessage = "Customer code must not exceed 20 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\-]*$", ErrorMessage = "Customer code may only contain letters, digits and hyphens.")]
    public string? CustomerCode { get; set; }
}
