namespace Futurist.Repository.Command.AnlRmCommand;

public class GetFgPlanCostPriceCommand : BaseCommand
{
    public int Room { get; init; }
    public int VerId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
}
