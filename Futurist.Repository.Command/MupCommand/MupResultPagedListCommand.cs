using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupResultPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; init; } = new();
}