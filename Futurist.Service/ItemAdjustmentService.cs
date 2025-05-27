using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Futurist.Common.Helpers;
using Futurist.Repository.Command.ItemAdjustmentCommand;
using Futurist.Repository.Interface;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Command.ItemAdjustmentCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class ItemAdjustmentService : IItemAdjustmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<ItemAdjustmentService>();

    public ItemAdjustmentService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<ItemAdjustmentDto>>> GetItemAdjustmentListAsync(int room)
    {
        try
        {
            var command = new GetItemAdjustmentListCommand
            {
                Room = room
            };

            var itemAdjustments = await _unitOfWork.ItemAdjustmentRepository.GetItemAdjustmentListAsync(command);

            return new ServiceResponse<IEnumerable<ItemAdjustmentDto>>
            {
                Data = _mapper.MapToIEnumerableDto(itemAdjustments),
                Message = ServiceMessageConstants.ItemAdjustmentListFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "ItemAdjustmentListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<ItemAdjustmentDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemAdjustmentListNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetItemAdjustmentRoomIdsAsync()
    {
        try
        {
            var command = new GetItemAdjustmentRoomIdsCommand();

            var roomIds = await _unitOfWork.ItemAdjustmentRepository.GetItemAdjustmentRoomIdsAsync(command);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Data = roomIds,
                Message = ServiceMessageConstants.ItemAdjustmentRoomIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetItemAdjustmentRoomIdsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemAdjustmentRoomIdsNotFound
            };
        }
    }

    public async Task<ServiceResponse> ImportAsync(ImportCommand serviceCommand)
    {
        try
        {
            var createdDate = DateTime.UtcNow;
            var itemAdjustments = ExcelHelper.ParseExcel(serviceCommand.Stream, row => new ItemAdjustmentDto
            {
                Room = row.Cell(1).TryGetValue(out int room) ? room : 0,
                ItemId = row.Cell(2).Value.ToString(),
                ItemName = row.Cell(3).Value.ToString(),
                UnitId = row.Cell(4).Value.ToString(),
                ItemGroup = row.Cell(5).Value.ToString(),
                GroupProcurement = row.Cell(6).Value.ToString(),
                Price = row.Cell(7).TryGetValue(out decimal price) ? price : 0,
                CreatedBy = string.IsNullOrEmpty(serviceCommand.User) ? "Unknown" : serviceCommand.User,
                CreatedDate = createdDate
            }).ToArray();
            
            if (itemAdjustments.Length == 0)
            {
                throw new ServiceException(ServiceMessageConstants.ItemAdjustmentMustNotBeEmpty);
            }

            var errors = new List<string>();
            
            // check if room is empty
            if (itemAdjustments.Any(x => x.Room <= 0))
            {
                var roomErrors = itemAdjustments.Select((x, i) => new { x, i }).Where(x => x.x.Room < 1).Select(x => x.i).ToArray();
                errors.Add($"{ServiceMessageConstants.ItemAdjustmentRoomInvalid}. Rows: {string.Join(", ", roomErrors.Take(10))}{(roomErrors.Length > 10 ? "..." : "")}");
            }
            
            // check if itemId is empty
            if (itemAdjustments.Any(x => string.IsNullOrEmpty(x.ItemId)))
            {
                var itemIdErrors = itemAdjustments.Select((x, i) => new { x, i }).Where(x => string.IsNullOrEmpty(x.x.ItemId)).Select(x => x.i).ToArray();
                // replace with elipsis if rows more than 10
                errors.Add($"{ServiceMessageConstants.ItemAdjustmentItemIdInvalid}. Rows: {string.Join(", ", itemIdErrors.Take(10))}{(itemIdErrors.Length > 10 ? "..." : "")}");
            }
            
            // check if price is less or equal than 0
            if (itemAdjustments.Any(x => x.Price <= 0))
            {
                var priceErrors = itemAdjustments.Select((x, i) => new { x, i }).Where(x => x.x.Price <= 0).Select(x => x.i).ToArray();
                // replace with elipsis if rows more than 10
                errors.Add($"{ServiceMessageConstants.ItemAdjustmentPriceInvalid}. Rows: {string.Join(", ", priceErrors.Take(10))}{(priceErrors.Length > 10 ? "..." : "")}");
            }
            
            // check if there is more than one room distinctively
            var roomIds = itemAdjustments.Select(x => x.Room).Distinct().ToArray();
            if (roomIds.Length != 1)
            {
                errors.Add(ServiceMessageConstants.ItemAdjustmentMustBeInOneRoom);
            }
            
            if (errors.Count > 0)
            {
                return new ServiceResponse
                {
                    Errors = errors,
                    Message = ServiceMessageConstants.ItemAdjustmentUploadFailed
                };
            }
            
            //var transaction = _unitOfWork.BeginTransaction();
            var deleteCommand = new DeleteItemAdjustmentCommand
            {
                Room = roomIds.First(),
                //DbTransaction = transaction
            };
            
            await _unitOfWork.ItemAdjustmentRepository.DeleteItemAdjustmentAsync(deleteCommand);
            
            var insertCommand = new BulkInsertItemAdjustmentCommand
            {
                ItemAdjustments = _mapper.MapToIEnumerable(itemAdjustments),
                //DbTransaction = transaction
            };
            
            await _unitOfWork.ItemAdjustmentRepository.BulkInsertItemAdjustmentAsync(insertCommand);

            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }

            return new ServiceResponse
            {
                Message = ServiceMessageConstants.ItemAdjustmentUploaded
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "ImportAsync failed {@command}", e.Message);
            
            return new ServiceResponse
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemAdjustmentUploadFailed
            };
        }
    }
}