using Futurist.Domain.Common;

namespace Futurist.Repository.Command.ExchangeRateCommand;

public class GetExchangeRatePagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; set; } = new();
}