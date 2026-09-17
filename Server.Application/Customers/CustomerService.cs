using CustomerManagement.Shared.Common;
using CustomerManagement.Shared.DTOs;
using FluentValidation;
using Server.Application.Common.Exceptions;
using Server.Application.Common.Interfaces;
using Server.Domain.Common;
using Server.Domain.Entities;

namespace Server.Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IValidator<CreateCustomerRequest> _createValidator;

    public CustomerService(
        ICustomerRepository repository,
        IValidator<CreateCustomerRequest> createValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new AppValidationException(validationResult.Errors);
        }

        var customerCode = await ResolveCustomerCodeAsync(request.CustomerCode, cancellationToken);

        var customer = new Customer
        {
            CustomerCode = customerCode,
            FullName = request.FullName.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            DateOfBirth = request.DateOfBirth,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return customer.ToDto();
    }

    // Returns a trimmed, uniqueness-checked customer code when one is supplied
    // Otherwise auto-generates the next sequential "KH#####" code
    private async Task<string> ResolveCustomerCodeAsync(string? requestedCode, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(requestedCode))
        {
            var trimmed = requestedCode.Trim();
            if (await _repository.CustomerCodeExistsAsync(trimmed, cancellationToken))
            {
                throw new ConflictException($"Customer code '{trimmed}' is already in use.");
            }

            return trimmed;
        }

        var nextSequence = await _repository.GetNextCustomerCodeSequenceAsync(cancellationToken);
        return $"{CustomerCodeDefaults.AutoPrefix}{nextSequence:D5}";
    }

    public async Task<PagedResult<CustomerDto>> SearchAsync(CustomerQueryParameters query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _repository.SearchAsync(
            query.SearchField,
            query.SearchTerm,
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResult<CustomerDto>
        {
            Items = items.Select(c => c.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

}
