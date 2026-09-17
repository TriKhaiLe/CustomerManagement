using CustomerManagement.Shared.DTOs;
using Server.Domain.Entities;

namespace Server.Application.Customers;

public static class CustomerMappingExtensions
{
    public static CustomerDto ToDto(this Customer customer) => new()
    {
        Id = customer.Id,
        CustomerCode = customer.CustomerCode,
        FullName = customer.FullName,
        Email = customer.Email ?? string.Empty,
        PhoneNumber = customer.PhoneNumber,
        DateOfBirth = customer.DateOfBirth.HasValue
            ? customer.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
            : null,
        IsActive = customer.IsActive,
        CreatedAt = customer.CreatedAt
    };
}
