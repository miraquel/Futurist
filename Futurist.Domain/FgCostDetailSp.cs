namespace Futurist.Domain;

public class FgCostDetailSp
{
    public int Room { get; set; }
    public int RofoId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime RofoDate { get; set; }
    public decimal RofoQty { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string GroupSubstitusi { get; set; } = string.Empty;
    public string ItemAllocatedId { get; set; } = string.Empty;
    public string ItemAllocatedName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string InventBatch { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal RmPrice { get; set; }
    public decimal PmPrice { get; set; }
    public decimal StdCostPrice { get; set; }
    public string Source { get; set; } = string.Empty;
    public int RefId { get; set; }
    public decimal LatestPurchasePrice { get; set; }
}