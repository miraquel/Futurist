namespace Futurist.Repository.Command.AnlRmCommand;

public class GetBomStdVsActDetCommand : BaseCommand
{
    public int VerId { get; init; }
    public int Room { get; init; }
    public int Tahun { get; init; }
    public int Bulan { get; init; }
    public string ProductId { get; init; } = string.Empty;
}
