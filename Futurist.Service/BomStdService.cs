using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class BomStdService : IBomStdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public BomStdService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse> ProcessBomStdAsync(int roomId)
    {
        try
        {
            var response = await _unitOfWork.BomStdRepository.ProcessBomStdAsync(roomId, _unitOfWork.BeginTransaction());
            
            await _unitOfWork.CommitAsync();

            return new ServiceResponse
            {
                Message = response
            };
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackAsync();
            
            return new ServiceResponse
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<BomStdDto>>> BomErrorCheckAsync(int roomId)
    {
        var response = await _unitOfWork.BomStdRepository.BomErrorCheckAsync(roomId);
        
        return new ServiceResponse<IEnumerable<BomStdDto>>
        {
            Message = ServiceMessageConstants.BomStdFound,
            Data = _mapper.MapToIEnumerableDto(await _unitOfWork.BomStdRepository.BomErrorCheckAsync(roomId))
        };
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync()
    {
        var response = await _unitOfWork.BomStdRepository.GetRoomIdsAsync();
        
        return new ServiceResponse<IEnumerable<int>>
        {
            Message = ServiceMessageConstants.RoomIdsFound,
            Data = response
        };
    }
}