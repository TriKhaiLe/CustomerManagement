using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Shared.DTOs;

public class CreateCustomerRequest : CustomerRequestBase
{
    // This is optional; the server assigns one when blank, and it cannot be changed later.
    [StringLength(50, ErrorMessageResourceName = "CustomerCodeMaxLength", ErrorMessageResourceType = typeof(CustomerManagement.Shared.Resources.ValidationMessages))]
    public string? CustomerCode { get; set; }
}
