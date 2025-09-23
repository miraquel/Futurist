namespace Futurist.Repository.Command.FgCostCommand;

public class CalculateFgCostCommand : BaseCommand
{
    public int Room { get; init; }
    public int Timeout { get; init; }
}