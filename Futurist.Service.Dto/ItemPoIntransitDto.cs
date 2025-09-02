namespace Futurist.Service.Dto;

public class ItemPoIntransitDto
{
    public string Po { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
}