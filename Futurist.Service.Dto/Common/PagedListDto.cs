using Newtonsoft.Json;

namespace Futurist.Service.Dto.Common
{
    public class PagedListDto<T>
    {
        public PagedListDto(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Items { get; init; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int PageNumber { get; init; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int PageSize { get; init; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int TotalCount { get; init; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool HasPreviousPage => PageNumber > 1 && PageNumber <= TotalPages;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool HasNextPage => PageNumber < TotalPages;
    }
}