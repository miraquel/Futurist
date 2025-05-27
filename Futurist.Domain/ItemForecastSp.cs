using System.Data.SqlTypes;

namespace Futurist.Domain;

public class ItemForecastSp
{
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string VtaMpSubstitusiGroupId { get; set; } = string.Empty;
    public string GroupProcurement { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime LatestPurchaseDate { get; set; } = SqlDateTime.MinValue.Value;
    public DateTime ForecastDate { get; set; } = SqlDateTime.MinValue.Value;
    public decimal ForecastPrice { get; set; }
    public decimal? ForcedPrice { get; set; }
}