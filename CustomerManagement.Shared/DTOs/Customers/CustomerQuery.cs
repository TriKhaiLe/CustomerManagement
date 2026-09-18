namespace CustomerManagement.Shared.DTOs;

public class CustomerQuery
{
    // PageNumber is one-based.
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public CustomerSearchField? SearchField { get; set; }

    // SearchField is ignored without a term.
    public string? SearchTerm { get; set; }
}
