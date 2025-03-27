namespace Futurist.Domain;

public class ExchangeRateSp
{
    public int RecId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public decimal ExchangeRate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}