namespace Futurist.Domain;
public class ItemPoIntransitVer
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string Po { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Room { get; set; }
    public int VerId { get; set; }
}
