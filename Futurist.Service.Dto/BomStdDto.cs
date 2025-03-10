namespace Futurist.Service.Dto;

public class BomStdDto
{
    public int Room { get; set; }
    public string BomId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal BomQty { get; set; }
    public decimal BomQtySerie { get; set; }
    public int LineType { get; set; }
    public string SubBomId { get; set; } = string.Empty;
    public string Ref { get; set; } = string.Empty;
    public int Level { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int RecId { get; set; }
}