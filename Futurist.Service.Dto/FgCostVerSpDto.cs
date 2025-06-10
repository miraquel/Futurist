namespace Futurist.Service.Dto;

public class FgCostVerSpDto
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int RofoId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal UnitInKg { get; set; }
    public DateTime RofoDate { get; set; }
    public decimal RofoQty { get; set; }
    public decimal Yield { get; set; }
    public decimal RmPrice { get; set; }
    public decimal PmPrice { get; set; }
    public decimal CostRmPmY { get; set; }
    public decimal StdCostPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalesPrice { get; set; }
    public string PreviousCalc { get; set; } = string.Empty;
    public decimal RofoQtyPrev { get; set; }
    public decimal SalesPricePrev { get; set; }
    public decimal YieldPrev { get; set; }
    public decimal RmPrev { get; set; }
    public decimal PmPrev { get; set; }
    public decimal CostRmPmYPrev { get; set; }
    public decimal StdCostPrev { get; set; }
    public decimal CostPricePrev { get; set; }
    public decimal DeltaAbsolute { get; set; }
}