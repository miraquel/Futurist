﻿namespace Futurist.Domain;
public class RofoVer
{
    public int Room { get; set; }
    public DateTime RofoDate { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal QtyRem { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int RecId { get; set; }
    public int VerId { get; set; }
}
