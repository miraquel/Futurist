namespace Futurist.Repository.Command.MaterialPlanCommand;

public class ProcessMaterialPlanCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public int Timeout { get; init; }
}
