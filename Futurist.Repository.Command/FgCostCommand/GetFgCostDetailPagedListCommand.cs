using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.FgCostCommand;

public class GetFgCostDetailPagedListCommand : BaseCommand
{
    public PagedListRequest<FgCostDetailSp> PagedListRequest { get; init; } = new();
}