namespace Futurist.Repository.Command.BomPlanCommand;

public class GetBomPlanMonthsCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
    public int Year { get; init; }
}
