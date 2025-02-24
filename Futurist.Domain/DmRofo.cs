namespace Futurist.Domain;
public class DmRofo
{
    public int Room { get; set; }
    public DateTime RofoDate { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public int RecId { get; set; }
}
