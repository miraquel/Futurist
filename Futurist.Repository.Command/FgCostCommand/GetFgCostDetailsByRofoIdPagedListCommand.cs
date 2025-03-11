using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetFgCostDetailsByRofoIdPagedListCommand : BaseCommand
{
    public PagedListRequest<FgCostDetailSp> PagedListRequest { get; set; } = new();
}