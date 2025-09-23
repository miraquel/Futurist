namespace Futurist.Domain;

public class ScmReport
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
    public decimal? SalesPrice { get; set; }
    public decimal? SalesPriceInKg { get; set; }
    public decimal? LinePercent { get; set; }
    public decimal? SumLineDiscMst { get; set; }
    public decimal? SalesAmount { get; set; }
    public decimal? RmpmAmount { get; set; }
    public decimal? StdCost { get; set; }
    public decimal? RmpmPercentage { get; set; }
    public decimal? Cogs { get; set; }
    public decimal? CogsPercentage { get; set; }
    public decimal? Margin { get; set; }
}