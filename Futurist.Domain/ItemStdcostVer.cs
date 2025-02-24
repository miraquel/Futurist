namespace Futurist.Domain;
public class ItemStdcostVer
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Room { get; set; }
    public int VerId { get; set; }
}
