using Futurist.Repository.Command.ReportVersionCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class ReportVersionService : IReportVersionService
{   
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public ReportVersionService(MapperlyMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<IEnumerable<FgCostVerSpDto>>> GetAllFgCostVerAsync(int room, int verId)
    {
        try
        {
            var command = new GetAllFgCostVerCommand
            {
                Room = room,
                VerId = verId
            };
            var fgCostVerList = await _unitOfWork.ReportVersionRepository.GetAllFgCostVerAsync(command);

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

    public async Task<ServiceResponse<IEnumerable<FgCostVerDetailSpDto>>> GetAllFgCostVerDetailsByRofoIdAsync(int rofoId, int verId)
    {
        try
        {
            var command = new GetAllFgCostVerDetailsCommand
            {
                RofoId = rofoId,
                VerId = verId
            };
            var fgCostVerDetailsList = await _unitOfWork.ReportVersionRepository.GetAllFgCostVerDetailsByRofoIdAsync(command);

            return new ServiceResponse<IEnumerable<FgCostVerDetailSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(fgCostVerDetailsList),
                Message = ServiceMessageConstants.FgCostVerDetailsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<FgCostVerDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostVerDetailsNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<MupVerSpDto>>> GetAllMupVerAsync(int room, int verId)
    {
        try
        {
            var command = new GetAllMupVerCommand
            {
                Room = room,
                VerId = verId
            };
            var mupVerList = await _unitOfWork.ReportVersionRepository.GetAllMupVerAsync(command);

            return new ServiceResponse<IEnumerable<MupVerSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(mupVerList),
                Message = ServiceMessageConstants.MupVerFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<MupVerSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.MupVerNotFound
            };
        }
    }

    public async Task<ServiceResponse<SpTaskDto>> InsertVersionAsync(int room, string notes)
    {
        try
        {
            var command = new InsertVersionCommand
            {
                Room = room,
                Notes = notes
            };
            var spTask = await _unitOfWork.ReportVersionRepository.InsertVersionAsync(command) ?? throw new NullReferenceException("InsertVersion processing does not return any result");

            _unitOfWork.CurrentTransaction?.Commit();

            return new ServiceResponse<SpTaskDto>
            {
                Data = _mapper.MapToDto(spTask),
                Message = ServiceMessageConstants.FgCostVerInserted
            };
        }
        catch (Exception e)
        {
            _unitOfWork.CurrentTransaction?.Rollback();
            
            return new ServiceResponse<SpTaskDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostVerNotInserted
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetVersionRoomIdsAsync()
    {
        try
        {
            var command = new GetVersionRoomIdsCommand();
            var roomIds = await _unitOfWork.ReportVersionRepository.GetVersionRoomIdsAsync(command);

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

    public async Task<ServiceResponse<IEnumerable<VersionsDto>>> GetVersionsAsync(int room)
    {
        try
        {
            var command = new GetVersionsCommand
            {
                Room = room
            };
            var versions = await _unitOfWork.ReportVersionRepository.GetVersionsAsync(command);

            return new ServiceResponse<IEnumerable<VersionsDto>>
            {
                Data = _mapper.MapToIEnumerableDto(versions),
                Message = ServiceMessageConstants.VersionsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<VersionsDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.VersionsNotFound
            };
        }
    }
}