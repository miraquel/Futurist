namespace Futurist.Domain;
public class Currency
{
    public int RecId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime CurrencyDate { get; set; }
    public decimal CurrencyRate { get; set; }
}
