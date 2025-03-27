using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetSummaryFgCostPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; init; } = new();
}