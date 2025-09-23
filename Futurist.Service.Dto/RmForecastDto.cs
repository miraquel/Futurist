namespace Futurist.Service.Dto;

public class RmForecastDto
{
    public int Year { get; set; }
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string GroupSubstitusi { get; set; } = string.Empty;
    public string GroupProcurement { get; set; } = string.Empty;
    public DateTime? LatestPurchaseDate { get; set; }
    public decimal LatestPurchasePrice { get; set; }
    public decimal January { get; set; }
    public decimal February { get; set; }
    public decimal March { get; set; }
    public decimal April { get; set; }
    public decimal May { get; set; }
    public decimal June { get; set; }
    public decimal July { get; set; }
    public decimal August { get; set; }
    public decimal September { get; set; }
    public decimal October { get; set; }
    public decimal November { get; set; }
    public decimal December { get; set; }
}