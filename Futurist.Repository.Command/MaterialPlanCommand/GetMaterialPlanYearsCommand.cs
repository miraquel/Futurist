namespace Futurist.Repository.Command.MaterialPlanCommand;

public class GetMaterialPlanYearsCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int VerId { get; init; }
}
