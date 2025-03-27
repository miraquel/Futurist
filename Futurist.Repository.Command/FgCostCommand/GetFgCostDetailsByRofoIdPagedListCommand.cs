using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetFgCostDetailsByRofoIdPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; set; } = new();
}