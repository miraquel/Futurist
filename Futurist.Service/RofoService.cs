using Futurist.Common.Helpers;
using Futurist.Repository.Command.RofoCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;

namespace Futurist.Service;

public class RofoService : IRofoService
{
    private readonly MapperlyMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RofoService(MapperlyMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<PagedListDto<RofoDto>>> GetPagedListAsync(PagedListRequestDto<RofoDto> pagedListRequest)
    {
        var filter = _mapper.MapToPagedListRequest(pagedListRequest);
        
        var command = new GetRofoPagedListCommand
        {
            PagedListRequest = filter
        };
        
        var result = await _unitOfWork.RofoRepository.GetRofoPagedListAsync(command);
            
        return new ServiceResponse<PagedListDto<RofoDto>>
        {
            Message = ServiceMessageConstants.RofoFound,
            Data = _mapper.MapToPagedListDto(result)
        };
    }

    public async Task<ServiceResponse<RofoDto>> GetByIdAsync(int rofoId)
    {
        var command = new GetRofoByIdCommand
        {
            Id = rofoId
        };
        
        var result = await _unitOfWork.RofoRepository.GetRofoByIdAsync(command);
        
        if (result == null)
        {
            throw new ServiceException(ServiceMessageConstants.RofoNotFound);
        }
        
        return new ServiceResponse<RofoDto>
        {
            Message = ServiceMessageConstants.RofoFound,
            Data = _mapper.MapToDto(result)
        };
    }

    public async Task<ServiceResponse> ImportAsync(Stream stream)
    {
        var rofoDtos = ExcelHelper.ParseExcel(stream, row =>
        {
            var rofoDto = new RofoDto
            {
                Room = row.Cell(1).TryGetValue(out int room) ? room : 0,
                RofoDate = row.Cell(2).TryGetValue(out DateTime rofoDate) ? rofoDate : DateTime.MinValue,
                ItemId = row.Cell(3).Value.ToString(),
                ItemName = row.Cell(4).Value.ToString(),
                Qty = row.Cell(5).TryGetValue(out decimal qty) ? qty : 0,
            };
            return rofoDto;
        }).ToArray();
            
        if (rofoDtos.Length == 0)
        {
            throw new ServiceException(ServiceMessageConstants.RofoMustNotBeEmpty);
        }
        
        var errors = new List<string>();
        if (rofoDtos.Any(x => x.Room == 0))
        {
            var roomErrors = rofoDtos.Where(x => x.Room == 0).Select(x => x.RecId).ToArray();
            errors.Add($"{ServiceMessageConstants.RofoRoomInvalid}. Rows: {string.Join(", ", roomErrors.Length > 10 ? $"{roomErrors.Take(10)}..." : roomErrors)}");
        }
        
        if (rofoDtos.Any(x => x.RofoDate == DateTime.MinValue))
        {
            var rofoDateErrors = rofoDtos.Where(x => x.RofoDate == DateTime.MinValue).Select(x => x.RecId).ToArray();
            errors.Add($"{ServiceMessageConstants.RofoDateInvalid}. Rows: {string.Join(", ", rofoDateErrors.Length > 10 ? $"{rofoDateErrors.Take(10)}..." : rofoDateErrors)}");
        }
        
        if (rofoDtos.Any(x => string.IsNullOrWhiteSpace(x.ItemId)))
        {
            var itemIdErrors = rofoDtos.Where(x => string.IsNullOrWhiteSpace(x.ItemId)).Select(x => x.RecId).ToArray();
            errors.Add($"{ServiceMessageConstants.RofoItemIdInvalid}. Rows: {string.Join(", ", itemIdErrors.Length > 10 ? $"{itemIdErrors.Take(10)}..." : itemIdErrors)}");
        }
        
        if (rofoDtos.Any(x => x.Qty <= 0))
        {
            var qtyErrors = rofoDtos.Where(x => x.Qty <= 0).Select(x => x.RecId).ToArray();
            errors.Add($"{ServiceMessageConstants.RofoQtyInvalid}. Rows: {string.Join(", ", qtyErrors.Length > 10 ? $"{qtyErrors.Take(10)}..." : qtyErrors)}");
        }
        
        if (errors.Count != 0)
        {
            return new ServiceResponse
            {
                Errors = errors
            };
        }
        
        var rofoRooms = rofoDtos.Select(x => x.Room).Distinct().ToArray();
        if (rofoRooms.Length != 1)
        {
            throw new ServiceException(ServiceMessageConstants.RofoMustBeInOneRoom);
        }
        
        var transaction = _unitOfWork.BeginTransaction();
        
        var deleteCommand = new DeleteRofoByRoomCommand
        {
            RoomId = rofoRooms.First(),
            DbTransaction = transaction
        };
        await _unitOfWork.RofoRepository.DeleteRofoByRoomAsync(deleteCommand);
        
        var bulkInsertCommand = new BulkInsertRofoCommand
        {
            Rofos = _mapper.MapToIEnumerable(rofoDtos),
            DbTransaction = transaction
        };
        await _unitOfWork.RofoRepository.BulkInsertRofoAsync(bulkInsertCommand);
        
        await _unitOfWork.CommitAsync();
        
        return new ServiceResponse
        {
            Message = ServiceMessageConstants.RofoImported
        };
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRofoRoomIdsAsync()
    {
        try
        {
            var command = new GetRofoRoomIdsCommand();
            var response = await _unitOfWork.RofoRepository.GetRofoRoomIdsAsync(command);

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