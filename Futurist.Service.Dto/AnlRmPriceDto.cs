namespace Futurist.Service.Dto;

public class AnlRmPriceDto
{
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string GroupSubstitusi { get; set; } = string.Empty;
    public string GroupProcurement { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public decimal PlanQty { get; set; }
    public decimal PlanValue { get; set; }
    public decimal PlanPrice { get; set; }
    public decimal ActQty { get; set; }
    public decimal ActValue { get; set; }
    public decimal ActPrice { get; set; }
    public decimal Cont { get; set; }
    public decimal Ap { get; set; }
}