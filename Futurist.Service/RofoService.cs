using Futurist.Common.Helpers;
using Futurist.Repository.Command.RofoCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Command.RofoCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class RofoService : IRofoService
{
    private readonly MapperlyMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger = Log.ForContext<RofoService>();

    public RofoService(MapperlyMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResponse<PagedListDto<RofoDto>>> GetPagedListAsync(PagedListRequestDto pagedListRequest)
    {
        try
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
        catch (Exception e)
        {
            _logger.Error(e, "GetPagedListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<RofoDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RofoNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<RofoDto>>> GetRofoListAsync(int room)
    {
        try
        {
            var command = new GetRofoListCommand
            {
                Room = room
            };

            var result = await _unitOfWork.RofoRepository.GetRofoListAsync(command);
            
            return new ServiceResponse<IEnumerable<RofoDto>>
            {
                Message = ServiceMessageConstants.RofoFound,
                Data = _mapper.MapToIEnumerableDto(result)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetRofoListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<RofoDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RofoNotFound
            };
        }
    }

    public async Task<ServiceResponse<RofoDto>> GetByIdAsync(int rofoId)
    {
        try
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
        catch (Exception e)
        {
            _logger.Error(e, "GetRofoByIdAsync failed {@command}", e.Message);
            
            return new ServiceResponse<RofoDto>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RofoNotFound
            };
        }
    }

    public async Task<ServiceResponse<int>> ImportAsync(ImportCommand serviceCommand)
    {
        try
        {
            var createdDate = DateTime.UtcNow;
            var rofoDtos = ExcelHelper.ParseExcel(serviceCommand.Stream, row => new RofoDto
            {
                Room = row.Cell(1).TryGetValue(out int room) ? room : 0,
                RofoDate = row.Cell(2).TryGetValue(out DateTime rofoDate) ? rofoDate : DateTime.MinValue,
                ItemId = row.Cell(3).Value.ToString(),
                ItemName = row.Cell(4).Value.ToString(),
                Qty = row.Cell(5).TryGetValue(out decimal qty) ? qty : 0,
                CreatedBy = string.IsNullOrEmpty(serviceCommand.User) ? "Unknown" : serviceCommand.User,
                CreatedDate = createdDate,
            }).ToArray();

            if (rofoDtos.Length == 0)
            {
                throw new ServiceException(ServiceMessageConstants.RofoMustNotBeEmpty);
            }

            var errors = new List<string>();
            if (rofoDtos.Any(x => x.Room == 0))
            {
                var roomErrors = rofoDtos.Select((x, i) => new { x, i }).Where(x => x.x.Room == 0).Select(x => x.i)
                    .ToArray();
                errors.Add($"{ServiceMessageConstants.RofoRoomInvalid}. Rows: {string.Join(", ", roomErrors)}");
            }

            if (rofoDtos.Any(x => x.RofoDate == DateTime.MinValue))
            {
                var rofoDateErrors = rofoDtos.Select((x, i) => new { x, i })
                    .Where(x => x.x.RofoDate == DateTime.MinValue).Select(x => x.i).ToArray();
                errors.Add($"{ServiceMessageConstants.RofoDateInvalid}. Rows: {string.Join(", ", rofoDateErrors)}");
            }

            if (rofoDtos.Any(x => string.IsNullOrWhiteSpace(x.ItemId)))
            {
                var itemIdErrors = rofoDtos.Select((x, i) => new { x, i })
                    .Where(x => string.IsNullOrWhiteSpace(x.x.ItemId)).Select(x => x.i).ToArray();
                errors.Add($"{ServiceMessageConstants.RofoItemIdInvalid}. Rows: {string.Join(", ", itemIdErrors)}");
            }

            if (rofoDtos.Any(x => x.Qty <= 0))
            {
                var qtyErrors = rofoDtos.Select((x, i) => new { x, i }).Where(x => x.x.Qty <= 0).Select(x => x.i)
                    .ToArray();
                errors.Add($"{ServiceMessageConstants.RofoQtyInvalid}. Rows: {string.Join(", ", qtyErrors)}");
            }

            if (errors.Count != 0)
            {
                return new ServiceResponse<int>
                {
                    Errors = errors,
                    Message = ServiceMessageConstants.RofoImportFailed
                };
            }

            var rofoRooms = rofoDtos.Select(x => x.Room).Distinct().ToArray();
            if (rofoRooms.Length != 1)
            {
                throw new ServiceException(ServiceMessageConstants.RofoMustBeInOneRoom);
            }

            //var transaction = _unitOfWork.BeginTransaction();

            var deleteCommand = new DeleteRofoByRoomCommand
            {
                RoomId = rofoRooms.First(),
                //DbTransaction = transaction
            };
            await _unitOfWork.RofoRepository.DeleteRofoByRoomAsync(deleteCommand);

            var bulkInsertCommand = new BulkInsertRofoCommand
            {
                Rofos = _mapper.MapToIEnumerable(rofoDtos),
                //DbTransaction = transaction
            };
            await _unitOfWork.RofoRepository.BulkInsertRofoAsync(bulkInsertCommand);

            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }

            _logger.Information("Rofo {@command}", bulkInsertCommand);

            return new ServiceResponse<int>
            {
                Message = ServiceMessageConstants.RofoImported,
                Data = rofoRooms.First()
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "ImportAsync failed {@command}", e.Message);
            
            return new ServiceResponse<int>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RofoImportFailed
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetRofoRoomIdsAsync()
    {
        try
        {
            var command = new GetRofoRoomIdsCommand();
            var response = await _unitOfWork.RofoRepository.GetRofoRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Message = ServiceMessageConstants.RofoRoomIdsFound,
                Data = response
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetRofoRoomIdsAsync failed {@command}", e.Message);
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.RofoRoomIdsNotFound
            };
        }
    }
}