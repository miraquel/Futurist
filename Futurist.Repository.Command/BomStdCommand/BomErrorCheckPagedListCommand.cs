using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.BomStdCommand;

public class BomErrorCheckPagedListCommand : BaseCommand
{
    public PagedListRequest<BomStd> PagedListRequest { get; init; } = new();
}