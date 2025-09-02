namespace Futurist.Domain;

public class AnlRmPriceGroup
{
    // false,1,Room,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,2,VerId,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,3,Year,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,4,Month,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,38,4,,
    // false,5,GroupSubstitusi,true,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,6,UnitId,true,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,7,PlanQty,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,8,PlanValue,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,9,PlanPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,10,ActQty,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,11,ActValue,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,1,true,1,108,17,,
    // false,12,ActPrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,13,Cont,true,108,"numeric(38,6)",17,38,6,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,14,A/P,true,108,"numeric(38,6)",17,38,6,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    
    public int Room { get; set; }
    public int VerId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string GroupSubstitusi { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public decimal PlanQty { get; set; }
    public decimal PlanValue { get; set; }
    public decimal PlanPrice { get; set; }
    public decimal ActQty { get; set; }
    public decimal ActValue { get; set; }
    public decimal ActPrice { get; set; }
    public decimal Cont { get; set; }
    public decimal Ap { get; set; }
}