namespace Futurist.Repository.Command.BomStdCommand;

public class ProcessBomStdCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int Timeout { get; init; }
}