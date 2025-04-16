using Futurist.Repository.Command.FgCostVerCommand;
using Futurist.Repository.Interface;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class FgCostVerService : IFgCostVerService
{   
    private readonly IFgCostVerRepository _fgCostVerRepository;
    private readonly MapperlyMapper _mapper;

    public FgCostVerService(IFgCostVerRepository fgCostVerRepository, MapperlyMapper mapper)
    {
        _fgCostVerRepository = fgCostVerRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<FgCostVerSpDto>>> GetAllFgCostVerAsync(int room)
    {
        try
        {
            var command = new GetAllFgCostVerCommand
            {
                Room = room
            };
            var fgCostVerList = await _fgCostVerRepository.GetAllFgCostVerAsync(command);

            return new ServiceResponse<IEnumerable<FgCostVerSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(fgCostVerList),
                Message = ServiceMessageConstants.FgCostVerFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<FgCostVerSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostVerNotFound
            };
        }
    }

    public async Task<ServiceResponse<SpTaskDto>> InsertFgCostVerAsync(int room, string notes)
    {
        try
        {
            var command = new InsertFgCostVerCommand
            {
                Room = room,
                Notes = notes
            };
            var spTask = await _fgCostVerRepository.InsertFgCostVerAsync(command) ?? throw new NullReferenceException("InsertFgCostVer processing does not return any result");

            return new ServiceResponse<SpTaskDto>
            {
                Data = _mapper.MapToDto(spTask),
                Message = ServiceMessageConstants.FgCostVerInserted
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<SpTaskDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostVerNotInserted
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetFgCostVerRoomIdsAsync()
    {
        try
        {
            var command = new GetFgCostVerRoomIdsCommand();
            var roomIds = await _fgCostVerRepository.GetFgCostVerRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = roomIds,
                Message = ServiceMessageConstants.FgCostVerRoomIdsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostVerRoomIdsNotFound
            };
        }
    }
}