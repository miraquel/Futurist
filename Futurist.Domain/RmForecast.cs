namespace Futurist.Domain;

public class RmForecast
{
    // false,1,Year,true,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,38,4,,
    // false,2,Room,false,56,int,4,10,0,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,56,4,,
    // false,3,ItemId,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,1,false,1,231,40,13632521,52
    // false,4,ItemName,true,231,nvarchar(4000),8000,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,231,8000,13632521,52
    // false,5,UnitId,true,231,nvarchar(10),20,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,20,13632521,52
    // false,6,GroupSubstitusi,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,231,40,13632521,52
    // false,7,GroupProcurement,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,231,40,13632521,52
    // false,8,LatestPurchaseDate,false,61,datetime,8,23,3,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,61,8,,
    // false,9,LatestPurchasePrice,true,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,10,January,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,11,February,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,12,March,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,13,March,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,14,May,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,15,June,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,16,July,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,17,August,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,18,September,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,19,October,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,20,November,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    // false,21,December,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,false,true,false,,,1,108,17,,
    
    public int Year { get; set; }
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string UnitId { get; set; } = string.Empty;
    public string GroupSubstitusi { get; set; } = string.Empty;
    public string GroupProcurement { get; set; } = string.Empty;
    public DateTime? LatestPurchaseDate { get; set; }
    public decimal LatestPurchasePrice { get; set; }
    public decimal January { get; set; }
    public decimal February { get; set; }
    public decimal March { get; set; }
    public decimal April { get; set; }
    public decimal May { get; set; }
    public decimal June { get; set; }
    public decimal July { get; set; }
    public decimal August { get; set; }
    public decimal September { get; set; }
    public decimal October { get; set; }
    public decimal November { get; set; }
    public decimal December { get; set; }
}