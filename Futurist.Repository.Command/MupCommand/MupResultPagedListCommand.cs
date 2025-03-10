using Futurist.Domain;
using Futurist.Domain.Common;

namespace Futurist.Repository.Command.MupCommand;

public class MupResultPagedListCommand : BaseCommand
{
    public PagedListRequest<MupSp> PagedListRequest { get; init; } = new();
}