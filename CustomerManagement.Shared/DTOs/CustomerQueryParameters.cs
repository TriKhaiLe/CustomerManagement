namespace CustomerManagement.Shared.DTOs;

public class CustomerQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public CustomerSearchField SearchField { get; set; } = CustomerSearchField.FullName;

    public string? SearchTerm { get; set; }

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }
}
