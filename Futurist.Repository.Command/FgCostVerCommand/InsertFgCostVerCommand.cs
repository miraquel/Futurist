using Futurist.Domain;

namespace Futurist.Repository.Command.FgCostVerCommand;

public class InsertFgCostVerCommand : BaseCommand
{
    public int Room { get; set; }
    public string Notes { get; set; } = string.Empty;
}