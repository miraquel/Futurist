namespace Futurist.Repository.Command.BomPlanCommand;

public class GetBomPlanYearsCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
}
