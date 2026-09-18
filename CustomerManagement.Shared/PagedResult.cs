namespace CustomerManagement.Shared;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();

    // PageNumber is one-based. Clients must render from TotalPages, HasPreviousPage, and HasNextPage,
    // rather than deriving them, so the server remains the single authority on paging.
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }
}
