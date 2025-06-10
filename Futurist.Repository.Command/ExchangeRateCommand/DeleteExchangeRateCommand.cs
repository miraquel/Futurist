namespace Futurist.Repository.Command.ExchangeRateCommand;

public class DeleteExchangeRateCommand : BaseCommand
{
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}