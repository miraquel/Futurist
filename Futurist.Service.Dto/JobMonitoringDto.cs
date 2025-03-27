using System.Data.SqlTypes;

namespace Futurist.Service.Dto;

public class JobMonitoringDto
{
    public int JobId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public DateTime TimeStart { get; set; } = SqlDateTime.MinValue.Value;
    public DateTime TimeEnd { get; set; } = SqlDateTime.MinValue.Value;
    public string Exception { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
    public string SourceContext { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    // duration
    public TimeSpan Duration => TimeEnd - TimeStart;
}