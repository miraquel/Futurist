using Futurist.Repository.Command.FgCostCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class FgCostService : IFgCostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;

    public FgCostService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<string?>> CalculateFgCostAsync(int roomId)
    {
        try
        {
            var transaction = _unitOfWork.BeginTransaction();
            var command = new CalculateFgCostCommand
            {
                RoomId = roomId,
                DbTransaction = transaction
            };
            var result = await _unitOfWork.FgCostRepository.CalculateFgCostAsync(command);
            await _unitOfWork.CommitAsync();
            
            return new ServiceResponse<string?>
            {
                Data = result,
                Message = ServiceMessageConstants.FgCostCalculated
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<string?>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostCalculationFailed
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostSpDto>>> GetSummaryFgCostAsync(int roomId)
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetSummaryFgCostAsync(new GetSummaryFgCostCommand { RoomId = roomId });
            return new ServiceResponse<IEnumerable<FgCostSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.SummaryFgCostFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<FgCostSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.SummaryFgCostNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<FgCostSpDto>>> GetSummaryFgCostPagedListAsync(PagedListRequestDto<FgCostSpDto> dto)
    {
        try
        {
            var command = new GetSummaryFgCostPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(dto)
            };
            var result = await _unitOfWork.FgCostRepository.GetSummaryFgCostPagedListAsync(command);
            return new ServiceResponse<PagedListDto<FgCostSpDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.SummaryFgCostFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<PagedListDto<FgCostSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.SummaryFgCostNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<FgCostDetailSpDto>>> GetFgCostDetailAsync(int roomId)
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailAsync(new GetFgCostDetailCommand { RoomId = roomId });
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<FgCostDetailSpDto>>> GetFgCostDetailPagedListAsync(PagedListRequestDto<FgCostDetailSpDto> dto)
    {
        try
        {
            var command = new GetFgCostDetailPagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(dto)
            };
            var result = await _unitOfWork.FgCostRepository.GetFgCostDetailPagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Data = _mapper.MapToPagedListDto(result),
                Message = ServiceMessageConstants.FgCostDetailFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<PagedListDto<FgCostDetailSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostDetailNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetFgCostRoomIdsAsync()
    {
        try
        {
            var result = await _unitOfWork.FgCostRepository.GetFgCostRoomIdsAsync(new GetFgCostRoomIdsCommand());
            return new ServiceResponse<IEnumerable<int>>
            {
                Data = result,
                Message = ServiceMessageConstants.FgCostRoomIdsFound
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.FgCostRoomIdsNotFound
            };
        }
    }
}