namespace Futurist.Service.Dto;

public class AnlKursDto
{
    public int VerId { get; set; }
    public int Room { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public decimal KursPlan { get; set; }
    public decimal KursAct { get; set; }
    public decimal Ap { get; set; }
}