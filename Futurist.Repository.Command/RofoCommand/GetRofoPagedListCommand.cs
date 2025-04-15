using Futurist.Domain.Common;

namespace Futurist.Repository.Command.RofoCommand;

public class GetRofoPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; init; } = new();
}
