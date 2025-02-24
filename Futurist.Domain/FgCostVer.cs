﻿namespace Futurist.Domain;
public class FgCostVer
{
    public int Room { get; set; }
    public int RofoId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime RofoDate { get; set; }
    public decimal QtyRofo { get; set; }
    public decimal Yield { get; set; }

    public decimal RmPrice { get; set; }

    public decimal PmPrice { get; set; }

    public decimal StdCostPrice { get; set; }

    public decimal CostPrice { get; set; }

    public int RecId { get; set; }

    public int VerId { get; set; }
}
