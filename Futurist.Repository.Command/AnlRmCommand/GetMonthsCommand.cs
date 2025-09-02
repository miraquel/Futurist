namespace Futurist.Repository.Command.AnlRmCommand;

public class GetMonthsCommand : BaseCommand
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
}