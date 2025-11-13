namespace Futurist.Service.Dto;

public class AnlCostPriceDto
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string GroupSubstitusi { get; set; } = string.Empty;
    public decimal PlanQty { get; set; }
    public decimal PlanPrice { get; set; }
    public decimal ActQty { get; set; }
    public decimal ActPrice { get; set; }
    public decimal Contr { get; set; }
    public decimal Ap { get; set; }
}