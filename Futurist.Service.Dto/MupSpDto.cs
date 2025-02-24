namespace Futurist.Service.Dto;

public class MupSpDto
{
    public int RofoId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime RofoDate { get; set; }
    public decimal QtyRofo { get; set; }
    public int MupItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string GroupSubstitusi { get; set; } = string.Empty;
    public int ItemAllocatedId { get; set; }
    public string ItemAllocatedName { get; set; } = string.Empty;
    public string InventBatch { get; set; } = string.Empty;
    public decimal AllocatedQty { get; set; }
    public string UnitId { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal RmPrice { get; set; }
    public decimal PmPrice { get; set; }
    public decimal StdCostPrice { get; set; }
    public string Source { get; set; } = string.Empty;
    public string RefId { get; set; } = string.Empty;
    public decimal LatestPurchasePrice { get; set; }
    public int Room { get; set; }
}