namespace Futurist.Repository.Command.ReportVersionCommand;

public class InsertVersionCommand : BaseCommand
{
    public int Room { get; set; }
    public string Notes { get; set; } = string.Empty;
}