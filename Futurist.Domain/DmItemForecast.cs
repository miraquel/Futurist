namespace Futurist.Domain;
public class DmItemForecast
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public DateTime ForecastDate { get; set; }
    public decimal ForecastPrice { get; set; }
    public decimal ForcedPrice { get; set; }
}
