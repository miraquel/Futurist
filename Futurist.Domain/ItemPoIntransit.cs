namespace Futurist.Domain;
public class ItemPoIntransit
{
    // false,1,Po,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,2,DeliveryDate,false,61,datetime,8,23,3,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,1,false,1,61,8,,
    // false,3,ItemId,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,4,ItemName,true,231,nvarchar(60),120,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,120,13632521,52
    // false,5,Qty,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    // false,6,Unit,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,7,CurrencyCode,false,231,nvarchar(20),40,0,0,SQL_Latin1_General_CP1_CI_AS,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,231,40,13632521,52
    // false,8,Price,false,108,"numeric(32,16)",17,32,16,,,,,,,,,,,false,false,false,,,,,,false,,true,false,false,,,1,108,17,,
    
    public string Po { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
