namespace Futurist.Service.Dto;

public class AnlFgPriceDto
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public decimal PlanQty { get; set; }
    public decimal PlanCostValue { get; set; }
    public decimal PlanCostPrice { get; set; }
    public decimal PlanRmPrice { get; set; }
    public decimal PlanPmPrice { get; set; }
    public decimal PlanStdCostPrice { get; set; }
    public decimal ActQty { get; set; }
    public decimal ActCostValue { get; set; }
    public decimal ActCostPrice { get; set; }
    public decimal ActRmPrice { get; set; }
    public decimal ActPmPrice { get; set; }
    public decimal ActStdCostPrice { get; set; }
    public decimal ActStdCostPriceLm { get; set; }
    public decimal Cont { get; set; }
    public decimal Ap { get; set; }
    public decimal ActCostPriceLm { get; set; }
    public decimal PlanNetSalesPriceIndex { get; set; }
    public decimal ActNetSalesPrice { get; set; }
    public decimal ActNetSalesPriceLm { get; set; }
    public decimal ActGrossSalesPrice { get; set; }
    public decimal ActGrossSalesPriceLm { get; set; }
    public decimal ActDiscPct { get; set; }
    public decimal ActDiscPctLm { get; set; }
}