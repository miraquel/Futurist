namespace Futurist.Repository.Command.MaterialActCommand;

public class ProcessMaterialActCommand : BaseCommand
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int Timeout { get; init; }
}
