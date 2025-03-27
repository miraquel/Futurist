using System.ComponentModel;
using System.Data.SqlTypes;

namespace Futurist.Service.Dto;

public class ExchangeRateSpDto
{
    [DisplayName("Rec ID")]
    public int RecId { get; set; }
    [DisplayName("Currency Code")]
    public string CurrencyCode { get; set; } = string.Empty;
    [DisplayName("Valid From")]
    public DateTime ValidFrom { get; set; } = SqlDateTime.MinValue.Value;
    [DisplayName("Valid To")]
    public DateTime ValidTo { get; set; } = SqlDateTime.MinValue.Value;
    [DisplayName("Exchange Rate")]
    public decimal ExchangeRate { get; set; }
    [DisplayName("Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    [DisplayName("Created Date")]
    public DateTime CreatedDate { get; set; } = SqlDateTime.MinValue.Value;
}