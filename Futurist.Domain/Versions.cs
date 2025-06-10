namespace Futurist.Domain;
public class Versions
{
    public int VerId { get; set; }
    public DateTime VerDate { get; set; }
    public int Room { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int Cancel { get; set; }
}
