namespace Futurist.Domain;

public class AnlFgPrice
{
    // false,1,Room,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,2,VerId,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,3,Year,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,4,Month,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,5,ItemId,true,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,6,ItemName,true,231,nvarchar(60),120,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,120,13632521,52
    // false,7,UnitId,true,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,8,PlanQty,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,9,PlanCostValue,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,10,PlanCostPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,11,PlanRmPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,12,PlanPmPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,13,PlanStdCostPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,14,ActQty,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,15,ActCostValue,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,1,true,1,108,17,,
    // false,16,ActCostPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,17,ActRmPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,18,ActPmPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,19,ActStdCostPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,20,ActStdCostPriceLm,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,21,Cont,true,108,"numeric(38,6)",17,38,6,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,22,A/P,true,108,"numeric(38,6)",17,38,6,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,23,PlanNetSalesPriceIndex,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,24,ActNetSalesPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,25,ActNetSalesPriceLm,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,26,ActGrossSalesPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,27,ActGrossSalesPriceLm,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,28,ActDisc%,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,29,ActDisc%Lm,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string ItemId { get; set; } = null!;
    public string ItemName { get; set; } = null!;
    public string UnitId { get; set; } = null!;
    public decimal PlanQty { get; set; }
    public decimal PlanCostValue { get; set; }
    public decimal PlanCostPrice { get; set; }
    public decimal PlanRmPrice { get; set; }
    public decimal PlanPmPrice { get; set; }
    public decimal PlanStdCostPrice { get; set; }
    public decimal ActQty { get; set; }
    public decimal ActCostValue { get; set; }
    public decimal ActCostPrice { get; set; }
    public decimal ActRmPrice { get; set; }
    public decimal ActPmPrice { get; set; }
    public decimal ActStdCostPrice { get; set; }
    public decimal ActStdCostPriceLm { get; set; }
    public decimal Cont { get; set; }
    public decimal Ap { get; set; }
    public decimal ActCostPriceLm { get; set; }
    public decimal PlanNetSalesPriceIndex { get; set; }
    public decimal ActNetSalesPrice { get; set; }
    public decimal ActNetSalesPriceLm { get; set; }
    public decimal ActGrossSalesPrice { get; set; }
    public decimal ActGrossSalesPriceLm { get; set; }
    public decimal ActDiscPct { get; set; }
    public decimal ActDiscPctLm { get; set; }
}