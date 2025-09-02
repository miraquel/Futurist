using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface ICurrentDataService
{
    Task<ServiceResponse<IEnumerable<ItemOnHandDto>>> GetItemOnHandAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<ItemPoIntransitDto>>> GetItemPoIntransitAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<ItemPagDto>>> GetItemPagAsync(CancellationToken cancellationToken);
}