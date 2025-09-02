namespace Futurist.Repository.Command.AnlRmCommand;

public class GetAnlFgPriceCommand : BaseCommand
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}