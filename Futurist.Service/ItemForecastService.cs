using System.Data.SqlTypes;
using Futurist.Common.Helpers;
using Futurist.Repository.Command.ItemForecast;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Command.ItemForecastCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class ItemForecastService : IItemForecastService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<ItemForecastService>();

    public ItemForecastService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<IEnumerable<ItemForecastSpDto>>> GetItemForecastListAsync(int room)
    {
        try
        {
            var command = new GetItemForecastListCommand
            {
                Room = room
            };

            var result = await _unitOfWork.ItemForecastRepository.GetItemForecastListAsync(command);

            return new ServiceResponse<IEnumerable<ItemForecastSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(result)
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "ItemAdjustmentListAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<ItemForecastSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemForecastNotFound
            };
        }
    }

    public async Task<ServiceResponse> ImportAsync(ImportCommand serviceCommand)
    {
        try
        {
            var createdDate = DateTime.UtcNow;
            var itemForecastCommands = ExcelHelper.ParseExcel(serviceCommand.Stream, row => new InsertItemForecastCommand
            {
                Room = row.Cell(1).TryGetValue(out int room) ? room : 0,
                ItemId = row.Cell(2).Value.ToString(),
                ForecastDate = row.Cell(9).TryGetValue(out DateTime validTo) ? validTo : DateTime.MinValue,
                ForcedPrice = row.Cell(11).TryGetValue(out decimal exchangeRate) ? exchangeRate : 0
            }).ToArray();
            
            if (itemForecastCommands.Length == 0)
            {
                throw new ServiceException(ServiceMessageConstants.ExchangeRateMustNotBeEmpty);
            }
            
            var errors = new List<string>();
            
            if (itemForecastCommands.Any(x => x.Room <= 0))
            {
                var roomErrors = itemForecastCommands.Select((x, i) => new { x, i }).Where(x => x.x.Room < 1).Select(x => x.i).ToArray();
                errors.Add($"{ServiceMessageConstants.ItemAdjustmentRoomInvalid}. Rows: {string.Join(", ", roomErrors.Take(10))}{(roomErrors.Length > 10 ? "..." : "")}");
            }
            
            if (itemForecastCommands.Any(x => string.IsNullOrWhiteSpace(x.ItemId)))
            {
                var itemIdErrors = itemForecastCommands.Select((x, i) => new { x, i }).Where(x => string.IsNullOrWhiteSpace(x.x.ItemId)).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ItemForecastItemIdInvalid}. Rows: {string.Join(", ", itemIdErrors.Take(10))}{(itemIdErrors.Length > 10 ? "..." : "")}");
            }
            
            if (itemForecastCommands.Any(x => x.ForecastDate == SqlDateTime.MinValue.Value))
            {
                var forecastDateErrors = itemForecastCommands.Select((x, i) => new { x, i }).Where(x => x.x.ForecastDate == DateTime.MinValue).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ItemForecastForecastDateInvalid}. Rows: {string.Join(", ", forecastDateErrors.Take(10))}{(forecastDateErrors.Length > 10 ? "..." : "")}");
            }
            
            if (itemForecastCommands.Any(x => x.ForcedPrice == 0))
            {
                var forcedPriceErrors = itemForecastCommands.Select((x, i) => new { x, i }).Where(x => x.x.ForcedPrice == 0).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ItemForecastForcedPriceInvalid}. Rows: {string.Join(", ", forcedPriceErrors.Take(10))}{(forcedPriceErrors.Length > 10 ? "..." : "")}");
            }
            
            if (errors.Count > 0)
            {
                return new ServiceResponse
                {
                    Errors = errors,
                    Message = ServiceMessageConstants.ItemForecastImportValidationFailed
                };
            }

            foreach (var command in itemForecastCommands)
            {
                await _unitOfWork.ItemForecastRepository.InsertItemForecastAsync(command);
            }

            _unitOfWork.CurrentTransaction?.Commit();

            return new ServiceResponse
            {
                Message = ServiceMessageConstants.ItemForecastImportSuccess
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "ItemForecastImportAsync failed {@command}", e.Message);
            
            return new ServiceResponse
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemForecastImportFailed
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<int>>> GetItemForecastRoomIdsAsync()
    {
        try
        {
            var command = new GetItemForecastRoomIdsCommand();

            var roomIds = await _unitOfWork.ItemForecastRepository.GetItemForecastRoomIdsAsync(command);

            return new ServiceResponse<IEnumerable<int>>
            {
                Data = roomIds,
                Message = ServiceMessageConstants.ItemForecastRoomIdsFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "GetItemForecastRoomIdsAsync failed {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<int>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ItemForecastRoomIdsNotFound
            };
        }
    }
}