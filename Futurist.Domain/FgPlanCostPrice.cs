namespace Futurist.Domain;

public class FgPlanCostPrice
{
    public int? VerId { get; set; }
    public int? Room { get; set; }
    public int? Tahun { get; set; }
    public int? Bulan { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? ValuePlan { get; set; }
    public decimal? ValueAct { get; set; }
}
