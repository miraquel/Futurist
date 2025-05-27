namespace Futurist.Repository.Command.ItemForecast;

public class InsertItemForecastCommand : BaseCommand
{
    // @Room int=5
    // ,@ItemId nvarchar(20) = '1000007'
    // ,@ForecastDate datetime = '1-Apr-2025'
    // ,@ForcedPrice numeric(32,16) =  12130
    
    public int Room { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public DateTime ForecastDate { get; set; } = new DateTime(2025, 4, 1);
    public decimal? ForcedPrice { get; set; }
}