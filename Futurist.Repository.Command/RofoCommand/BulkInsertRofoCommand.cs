namespace Futurist.Repository.Command.RofoCommand;

public class BulkInsertRofoCommand : BaseCommand
{
    public IEnumerable<Domain.Rofo> Rofos { get; init; } = new List<Domain.Rofo>();
}
