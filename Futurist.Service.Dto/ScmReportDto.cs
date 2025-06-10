namespace Futurist.Service.Dto;

public class ScmReportDto
{
    public string? PeriodDate { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceId { get; set; }
    public string? CustId { get; set; }
    public string? CustName { get; set; }
    public string? Divisi { get; set; }
    public string? BusinessUnit { get; set; }
    public string? ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? Brand { get; set; }
    public string? InventBatchId { get; set; }
    public decimal? Qty { get; set; }
    public decimal? QtyInKg { get; set; }
    public decimal? SalesAmount { get; set; }
    public decimal? RmpmAmount { get; set; }
    public decimal? StdCost { get; set; }
    public decimal? RmpmPercentage { get; set; }
}

