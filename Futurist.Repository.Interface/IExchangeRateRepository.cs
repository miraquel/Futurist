using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.ExchangeRateCommand;

namespace Futurist.Repository.Interface;

public interface IExchangeRateRepository
{
    Task BulkInsertExchangeRateAsync(BulkInsertExchangeRateCommand command);
    
    Task<PagedList<ExchangeRateSp>> GetExchangeRatePagedListAsync(GetExchangeRatePagedListCommand command);
    Task<ExchangeRateSp?> GetExchangeRateAsync(GetExchangeRateCommand command);
    Task<IEnumerable<ExchangeRateSp>> GetAllExchangeRateAsync(GetAllExchangeRateCommand command);
    Task DeleteExchangeRateAsync(DeleteExchangeRateCommand command);
}