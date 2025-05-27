namespace Futurist.Service.Dto;

public class FgCostSpDto
{
    public int Room { get; set; }
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
    public decimal RmPmYield { get; set; }
    public decimal StdCostPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SalesPriceIndex { get; set; }
}