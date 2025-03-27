using Futurist.Domain.Common;

namespace Futurist.Repository.Command.BomStdCommand;

public class BomErrorCheckPagedListCommand : BaseCommand
{
    public PagedListRequest PagedListRequest { get; init; } = new();
}