namespace Futurist.Domain;
public class DmItemPoIntransit
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string Po { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public decimal Price { get; set; }
}
