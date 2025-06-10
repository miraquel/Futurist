namespace Futurist.Web.Requests.ReportVersion;

public class InsertVersionRequest
{
    public int RoomId { get; set; }
    public string Notes { get; set; } = string.Empty;
}