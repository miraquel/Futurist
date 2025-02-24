namespace Futurist.Domain;
public class ItemYield
{
    public string ItemId { get; set; } = string.Empty;
    public decimal Yield { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int RecId { get; set; }
}
