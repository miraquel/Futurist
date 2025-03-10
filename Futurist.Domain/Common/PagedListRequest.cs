namespace Futurist.Domain.Common;
public class PagedListRequest<T> where T : new()
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = string.Empty;
    public bool IsSortAscending { get; set; }
    public string Search { get; set; } = string.Empty;
    public T Filter { get; set; } = new();
    public Dictionary<string, string> Filters { get; set; } = new();
}