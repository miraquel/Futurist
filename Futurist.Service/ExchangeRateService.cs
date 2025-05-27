using System.Data.SqlTypes;
using Futurist.Common.Helpers;
using Futurist.Repository.Command.ExchangeRateCommand;
using Futurist.Repository.UnitOfWork;
using Futurist.Service.Command.ExchangeRateCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Futurist.Service.Interface;
using Serilog;

namespace Futurist.Service;

public class ExchangeRateService : IExchangeRateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MapperlyMapper _mapper;
    private readonly ILogger _logger = Log.ForContext<ExchangeRateService>();

    public ExchangeRateService(IUnitOfWork unitOfWork, MapperlyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResponse> ImportAsync(ImportCommand serviceCommand)
    {
        try
        {
            var createdDate = DateTime.UtcNow;
            var exchangeRateSpDtos = ExcelHelper.ParseExcel(serviceCommand.Stream, row => new ExchangeRateSpDto
            {
                CurrencyCode = row.Cell(1).Value.ToString(),
                ValidFrom = row.Cell(2).TryGetValue(out DateTime validFrom) ? validFrom : DateTime.MinValue,
                ValidTo = row.Cell(3).TryGetValue(out DateTime validTo) ? validTo : DateTime.MinValue,
                ExchangeRate = row.Cell(4).TryGetValue(out decimal exchangeRate) ? exchangeRate : 0,
                CreatedBy = string.IsNullOrEmpty(serviceCommand.User) ? "Unknown" : serviceCommand.User,
                CreatedDate = createdDate
            }).ToArray();
            
            if (exchangeRateSpDtos.Length == 0)
            {
                throw new ServiceException(ServiceMessageConstants.ExchangeRateMustNotBeEmpty);
            }
            
            var errors = new List<string>();
            
            if (exchangeRateSpDtos.Any(x => string.IsNullOrWhiteSpace(x.CurrencyCode)))
            {
                var currencyCodeErrors = exchangeRateSpDtos.Select((x, i) => new { x, i }).Where(x => string.IsNullOrWhiteSpace(x.x.CurrencyCode)).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ExchangeRateCurrencyCodeInvalid}. Rows: {string.Join(", ", currencyCodeErrors)}");
            }
            
            if (exchangeRateSpDtos.Any(x => x.ValidFrom == SqlDateTime.MinValue.Value))
            {
                var validFromErrors = exchangeRateSpDtos.Select((x, i) => new { x, i }).Where(x => x.x.ValidFrom == SqlDateTime.MinValue.Value).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ExchangeRateValidFromInvalid}. Rows: {string.Join(", ", validFromErrors)}");
            }
            
            if (exchangeRateSpDtos.Any(x => x.ValidTo == SqlDateTime.MinValue.Value))
            {
                var validToErrors = exchangeRateSpDtos.Select((x, i) => new { x, i }).Where(x => x.x.ValidTo == SqlDateTime.MinValue.Value).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ExchangeRateValidToInvalid}. Rows: {string.Join(", ", validToErrors)}");
            }
            
            if (exchangeRateSpDtos.Any(x => x.ExchangeRate == 0))
            {
                var exchangeRateErrors = exchangeRateSpDtos.Select((x, i) => new { x, i }).Where(x => x.x.ExchangeRate == 0).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ExchangeRateInvalid}. Rows: {string.Join(", ", exchangeRateErrors)}");
            }
            
            if (exchangeRateSpDtos.Any(x => x.ValidFrom > x.ValidTo))
            {
                var validFromToErrors = exchangeRateSpDtos.Select((x, i) => new { x, i }).Where(x => x.x.ValidFrom > x.x.ValidTo).Select(x => x.i + 2).ToArray();
                errors.Add($"{ServiceMessageConstants.ExchangeRateValidFromToInvalid}. Rows: {string.Join(", ", validFromToErrors)}");
            }

            if (errors.Count != 0)
            {
                return new ServiceResponse
                {
                    Errors = errors
                };
            }
            
            foreach (var exchangeRateSpDto in exchangeRateSpDtos)
            {
                var getCommand = new GetExchangeRateCommand
                {
                    CurrencyCode = exchangeRateSpDto.CurrencyCode,
                    ValidFrom = exchangeRateSpDto.ValidFrom,
                    ValidTo = exchangeRateSpDto.ValidTo
                };
                var exchangeRate = await _unitOfWork.ExchangeRateRepository.GetExchangeRateAsync(getCommand);

                if (exchangeRate == null) continue;
                
                var deleteCommand = new DeleteExchangeRateCommand
                {
                    CurrencyCode = exchangeRate.CurrencyCode,
                    ValidFrom = exchangeRate.ValidFrom,
                    ValidTo = exchangeRate.ValidTo
                };
                    
                await _unitOfWork.ExchangeRateRepository.DeleteExchangeRateAsync(deleteCommand);
            }

            var command = new BulkInsertExchangeRateCommand
            {
                ExchangeRates = _mapper.MapToIEnumerable(exchangeRateSpDtos)
            };
            
            await _unitOfWork.ExchangeRateRepository.BulkInsertExchangeRateAsync(command);
            
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.CommitAsync();
            }
            
            _logger.Information("Exchange rate imported successfully");

            return new ServiceResponse
            {
                Message = ServiceMessageConstants.ExchangeRateImported
            };
        }
        catch (Exception e)
        {
            if (_unitOfWork.CurrentTransaction != null)
            {
                await _unitOfWork.RollbackAsync();
            }
            
            _logger.Error(e, "Exchange rate import failed: {@command}", e.Message);
            
            return new ServiceResponse
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ExchangeRateImportFailed
            };
        }
    }

    public async Task<ServiceResponse<PagedListDto<ExchangeRateSpDto>>> GetExchangeRatePagedListAsync(PagedListRequestDto pagedListRequestDto)
    {
        try
        {
            var command = new GetExchangeRatePagedListCommand
            {
                PagedListRequest = _mapper.MapToPagedListRequest(pagedListRequestDto)
            };
            
            var exchangeRatePagedList = await _unitOfWork.ExchangeRateRepository.GetExchangeRatePagedListAsync(command);
            
            return new ServiceResponse<PagedListDto<ExchangeRateSpDto>>
            {
                Data = _mapper.MapToPagedListDto(exchangeRatePagedList),
                Message = ServiceMessageConstants.ExchangeRateFound 
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "Exchange rate search failed: {@command}", e.Message);
            
            return new ServiceResponse<PagedListDto<ExchangeRateSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ExchangeRateNotFound
            };
        }
    }

    public async Task<ServiceResponse<IEnumerable<ExchangeRateSpDto>>> GetAllExchangeRateAsync()
    {
        try
        {
            var command = new GetAllExchangeRateCommand();
            
            var exchangeRate = await _unitOfWork.ExchangeRateRepository.GetAllExchangeRateAsync(command);

            return new ServiceResponse<IEnumerable<ExchangeRateSpDto>>
            {
                Data = _mapper.MapToIEnumerableDto(exchangeRate),
                Message = ServiceMessageConstants.ExchangeRateFound
            };
        }
        catch (Exception e)
        {
            _logger.Error(e, "Exchange rate search failed: {@command}", e.Message);
            
            return new ServiceResponse<IEnumerable<ExchangeRateSpDto>>
            {
                Errors = [e.Message],
                Message = ServiceMessageConstants.ExchangeRateNotFound
            };
        }
    }
}