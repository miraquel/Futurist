using System.Data.SqlTypes;

namespace Futurist.Domain;
public class Rofo
{
    public int Room { get; set; }
    public DateTime RofoDate { get; set; } = SqlDateTime.MinValue.Value;
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = SqlDateTime.MinValue.Value;
    public int RecId { get; set; }
}
