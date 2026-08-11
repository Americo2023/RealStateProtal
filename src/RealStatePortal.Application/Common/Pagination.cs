namespace RealStatePortal.Application.Common;

public sealed record PaginationRequest(int Page = 1, int PageSize = 20)
{
    public int ValidatedPage => Page < 1 ? 1 : Page;
    public int ValidatedPageSize => Math.Clamp(PageSize, 1, 100);
}

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}