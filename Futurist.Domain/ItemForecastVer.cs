namespace Futurist.Domain;
public class ItemForecastVer
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public DateTime ForecastDate { get; set; }
    public decimal ForecastPrice { get; set; }
    public decimal ForcedPrice { get; set; }
    public int Room { get; set; }
    public int VerId { get; set; }
}
