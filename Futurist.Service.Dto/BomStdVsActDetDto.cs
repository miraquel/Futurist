namespace Futurist.Service.Dto;

public class BomStdVsActDetDto
{
    public int? VerId { get; set; }
    public int? Room { get; set; }
    public int? Tahun { get; set; }
    public int? Bulan { get; set; }
    public string? ProductIdPlan { get; set; }
    public string? ProductNamePlan { get; set; }
    public string? ItemPlan { get; set; }
    public string? ItemNamePlan { get; set; }
    public string? UnitIdPlan { get; set; }
    public string? GroupPlan { get; set; }
    public decimal? QtyPlan { get; set; }
    public decimal? QtyInKgPlan { get; set; }
    public decimal? PricePlan { get; set; }
    public decimal? ValuePlan { get; set; }
    public decimal? YieldPlan { get; set; }
    public string? ItemIdAct { get; set; }
    public string? ItemNameAct { get; set; }
    public string? UnitIdAct { get; set; }
    public string? GroupSubstitusiAct { get; set; }
    public decimal? QtyAct { get; set; }
    public decimal? QtyInKgAct { get; set; }
    public decimal? PriceAct { get; set; }
    public decimal? ValueAct { get; set; }
}
