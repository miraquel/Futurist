namespace Futurist.Repository.Command.FgCostCommand;

public class CalculateFgCostCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int Timeout { get; init; }
}