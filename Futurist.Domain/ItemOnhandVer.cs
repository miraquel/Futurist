﻿namespace Futurist.Domain;
public class ItemOnhandVer
{
    public int RecId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string InventBatch { get; set; } = string.Empty;
    public DateTime ExpDate { get; set; }
    public string? PdsDispositionCode { get; set; }
    public decimal Qty { get; set; }
    public decimal? QtyRem { get; set; }
    public decimal Price { get; set; }
    public decimal RmPrice { get; set; }
    public decimal PmPrice { get; set; }
    public decimal StdcostPrice { get; set; }
    public int Room { get; set; }
    public int VerId { get; set; }
}
