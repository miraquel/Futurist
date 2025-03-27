﻿namespace Futurist.Service.Dto;

public class ItemAdjustmentDto
{
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string ItemGroup { get; set; } = string.Empty;
    public decimal Price { get; set; }
}