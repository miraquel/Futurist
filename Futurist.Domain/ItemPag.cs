namespace Futurist.Domain;
public class ItemPag
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string Pag { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
