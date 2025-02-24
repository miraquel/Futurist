namespace Futurist.Domain;
public class Mup
{
    public int Room { get; set; }
    public DateTime MupDate { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemGroup { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public int RofoId { get; set; }
    public int RecId { get; set; }
}
