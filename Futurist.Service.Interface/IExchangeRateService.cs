using Futurist.Service.Command.ExchangeRateCommand;
using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;

namespace Futurist.Service.Interface;

public interface IExchangeRateService
{
    Task<ServiceResponse> ImportAsync(ImportCommand serviceCommand);

    Task<ServiceResponse<PagedListDto<ExchangeRateSpDto>>> GetExchangeRatePagedListAsync(
        PagedListRequestDto pagedListRequestDto);
    
    Task<ServiceResponse<IEnumerable<ExchangeRateSpDto>>> GetAllExchangeRateAsync();
}