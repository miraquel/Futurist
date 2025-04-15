using System.ComponentModel;
using System.Data.SqlTypes;
using System.Runtime.Serialization;

namespace Futurist.Service.Dto;

public class MupSpDto
{
    [DisplayName("Room")]
    public int Room { get; set; }
    [DisplayName("Product ID")]
    public string ProductId { get; set; } = string.Empty;
    [DisplayName("Product Name")]
    public string ProductName { get; set; } = string.Empty;
    [DisplayName("Rofo Date")]
    public DateTime RofoDate { get; set; } = SqlDateTime.MinValue.Value;
    [DisplayName("Rofo Qty")]
    public decimal QtyRofo { get; set; }
    [DisplayName("Item ID")]
    public string ItemId { get; set; } = string.Empty;
    [DisplayName("Item Name")]
    public string ItemName { get; set; } = string.Empty;
    [DisplayName("Group Substitusi")]
    public string GroupSubstitusi { get; set; } = string.Empty;
    [DisplayName("Item Allocated ID")]
    public string ItemAllocatedId { get; set; } = string.Empty;
    [DisplayName("Item Allocated Name")]
    public string ItemAllocatedName { get; set; } = string.Empty;
    [DisplayName("Unit")]
    public string UnitId { get; set; } = string.Empty;
    [DisplayName("Batch")]
    public string InventBatch { get; set; } = string.Empty;
    [DisplayName("Qty")]
    public decimal Qty { get; set; }
    [DisplayName("Price")]
    public decimal Price { get; set; }
    [DisplayName("Source")]
    public string Source { get; set; } = string.Empty;
    [DisplayName("Ref ID")]
    public string RefId { get; set; } = string.Empty;
    [DisplayName("Latest Purchase Price")]
    public decimal LatestPurchasePrice { get; set; }
    [DisplayName("Gap")]
    public decimal Gap { get; set; }
    
    [DisplayName("Rofo ID"), IgnoreDataMember]
    public int RofoId { get; set; }
    [DisplayName("RM Price"), IgnoreDataMember]
    public decimal RmPrice { get; set; }
    [DisplayName("PM Price"), IgnoreDataMember]
    public decimal PmPrice { get; set; }
    [DisplayName("Std Cost Price"), IgnoreDataMember]
    public decimal StdCostPrice { get; set; }
    [DisplayName("MUP Date"), IgnoreDataMember]
    public DateTime MupDate { get; set; } = SqlDateTime.MinValue.Value;
}