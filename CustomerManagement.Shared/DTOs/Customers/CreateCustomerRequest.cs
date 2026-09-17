using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public class CreateCustomerRequest : CustomerRequestBase
{
    // This is optional; the server assigns one when blank, and it cannot be changed later.
    [StringLength(50, ErrorMessage = "Customer code cannot exceed 50 characters.")]
    public string? CustomerCode { get; set; }
}
