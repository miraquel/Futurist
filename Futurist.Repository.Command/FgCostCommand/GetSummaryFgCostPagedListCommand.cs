using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetSummaryFgCostPagedListCommand : BaseCommand
{
    public PagedListRequest<FgCostSp> PagedListRequest { get; init; } = new();
}