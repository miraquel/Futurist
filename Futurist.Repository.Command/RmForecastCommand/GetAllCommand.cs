namespace Futurist.Repository.Command.RmForecastCommand;

public class GetAllCommand : BaseCommand
{
    public int Room { get; set; }
    public int Year { get; set; }
}