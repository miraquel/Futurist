using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.RofoCommand;

public class GetRofoPagedListCommand : BaseCommand
{
    public PagedListRequest<Rofo> PagedListRequest { get; init; } = new();
}
