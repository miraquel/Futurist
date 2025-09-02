namespace Futurist.Domain;
public class ItemOnHand
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string InventBatch { get; set; } = string.Empty;
    public DateTime? ExpDate { get; set; }
    public string PdsDispositionCode { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string UnitId { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
