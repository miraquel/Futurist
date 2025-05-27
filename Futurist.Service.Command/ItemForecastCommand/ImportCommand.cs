namespace Futurist.Service.Command.ItemForecastCommand;

public class ImportCommand
{
    public Stream Stream { get; set; }
    public string User { get; set; } = string.Empty;
}