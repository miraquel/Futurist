using Futurist.Domain;

namespace Futurist.Repository.Command.ExchangeRateCommand;

public class BulkInsertExchangeRateCommand : BaseCommand
{
    public IEnumerable<ExchangeRateSp> ExchangeRates { get; set; } = new List<ExchangeRateSp>();
}