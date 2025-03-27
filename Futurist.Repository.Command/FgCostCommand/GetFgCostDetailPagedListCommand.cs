using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetFgCostDetailPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; init; } = new();
}