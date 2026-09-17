namespace CustomerManagement.Shared.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages
    {
        get
        {
            if (PageSize <= 0)
                return 0;

            return (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
