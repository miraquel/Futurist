using Futurist.Domain;

namespace Futurist.Repository.Command.RofoCommand;

public class BulkInsertRofoCommand : BaseCommand
{
    public IEnumerable<Rofo> Rofos { get; init; } = new List<Rofo>();
}
