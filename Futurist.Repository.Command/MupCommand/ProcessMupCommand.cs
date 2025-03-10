namespace Futurist.Repository.Command.MupCommand;

public class ProcessMupCommand : BaseCommand
{
    public int RoomId { get; init; }
    public int Timeout { get; init; }
}
