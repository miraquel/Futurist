namespace Futurist.Repository.Command.BomPlanCommand;

public class ProcessBomPlanCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public int Timeout { get; init; }
}
