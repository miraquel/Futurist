using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IAnlRmService
{
    Task<ServiceResponse<IEnumerable<AnlRmPriceDto>>> GetAnlRmpPrice(int room, int verId, int year, int month, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<AnlKursDto>>> GetAnlKursAsync(int version, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<AnlFgPriceDto>>> GetAnlFgPriceAsync(int room, int verId, int year, int month, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<AnlPmPriceDto>>> GetAnlPmPriceAsync(int room, int verId, int year, int month, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetRofoVerIdsAsync(int room, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetYearsAsync(int room, int verId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetMonthsAsync(int room, int verId, int year, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetVerIdsAsync(CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<AnlRmPriceGroupDto>>> GetAnlRmPriceGroupAsync(int room, int verId, int year, int month, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<AnlCostPriceDto>>> GetAnlCostPriceAsync(int room, int verId, int year, int month,
        string itemId, CancellationToken cancellationToken);
}