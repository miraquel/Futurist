namespace Futurist.Domain;
public class ItemTransVer
{
    public int RecId { get; set; }
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal RmPrice { get; set; }
    public decimal PmPrice { get; set; }
    public decimal StdCostPrice { get; set; }
    public string Source { get; set; } = string.Empty;
    public int RefId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal CurrencyRate { get; set; }
    public int VerId { get; set; }
}
