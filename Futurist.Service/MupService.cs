using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class MupService : IMupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public MupService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> ProcessMupAsync(int roomId)
    {
        try
        {
            var response = await _unitOfWork.MupRepository.ProcessMupAsync(roomId);

            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupProcessed,
                Data = _mapper.MapToIEnumerableDto(await _unitOfWork.MupRepository.ProcessMupAsync(roomId))
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupSpDto>>> MupResultAsync(int roomId)
    {
        try
        {
            var response = await _unitOfWork.MupRepository.MupResultAsync(roomId);

            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Message = ServiceMessageConstants.MupResultFound,
                Data = _mapper.MapToIEnumerableDto(await _unitOfWork.MupRepository.MupResultAsync(roomId))
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<MupSpDto>>
            {
                Errors = [e.Message]
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRoomIdsAsync()
    {
        try
        {
            var response = await _unitOfWork.MupRepository.GetRoomIdsAsync();

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.RoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message]
            };
        }
    }
}