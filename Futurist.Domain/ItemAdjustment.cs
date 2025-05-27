using System.Data.SqlTypes;

namespace Futurist.Domain;

public class ItemAdjustment
{
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string ItemGroup { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string GroupProcurement { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = "Unknown";
    public DateTime CreatedDate { get; set; } = SqlDateTime.MinValue.Value;
}