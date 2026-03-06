namespace Futurist.Repository.Command.MaterialPlanCommand;

public class GetMaterialPlanMonthsCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
    public int Year { get; init; }
}
