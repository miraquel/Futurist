using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IRmForecastService
{
    Task<ServiceResponse<IEnumerable<RmForecastDto>>> GetAllAsync(int room, int year,
        CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetYearsAsync(int room, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync(CancellationToken cancellationToken);
}