using Futurist.Repository.Command.RmForecastCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class RmForecastService : IRmForecastService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public RmForecastService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<RmForecastDto>>> GetAllAsync(int room, int year, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetAllCommand
            {
                Room = room,
                Year = year
            };
            var fgCostVerList = await _unitOfWork.RmForecastRepository.GetAllAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<RmForecastDto>>
            {
                Data = _mapper.MapToIEnumerableDto(fgCostVerList),
                Message = ServiceMessageConstants.RmForecastFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<RmForecastDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RmForecastNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetYearsAsync(int room, CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetYearsCommand
            {
                Room = room
            };
            var years = await _unitOfWork.RmForecastRepository.GetYearsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = years,
                Message = ServiceMessageConstants.RmForecastYearsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RmForecastYearsNotFound
            };
        }
    }
    
    public async Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var command = new GetRoomIdsCommand();
            var roomIds = await _unitOfWork.RmForecastRepository.GetRoomIdsAsync(command, cancellationToken);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = roomIds,
                Message = ServiceMessageConstants.RmForecastRoomIdsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RmForecastRoomIdsNotFound
            };
        }
    }
}