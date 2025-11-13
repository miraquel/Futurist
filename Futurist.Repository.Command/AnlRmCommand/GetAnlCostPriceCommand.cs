namespace Futurist.Repository.Command.AnlRmCommand;

public class GetAnlCostPriceCommand : BaseCommand
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string ItemId { get; set; } = string.Empty;
}